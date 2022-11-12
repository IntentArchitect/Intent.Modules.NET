using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Intent.Engine;
using Intent.Metadata.RDBMS.Api;
using Intent.Modelers.Domain.Api;
using Intent.Modelers.Services.Api;
using Intent.Modelers.Services.CQRS.Api;
using Intent.Modules.Application.MediatR.CRUD.Decorators;
using Intent.Modules.Application.MediatR.Templates;
using Intent.Modules.Application.MediatR.Templates.CommandHandler;
using Intent.Modules.Common.Templates;
using Intent.Modules.Entities.Repositories.Api.Templates.EntityRepositoryInterface;
using Intent.Modules.Metadata.RDBMS.Settings;

namespace Intent.Modules.Application.MediatR.CRUD.CrudStrategies
{
    class CreateImplementationStrategy : ICrudImplementationStrategy
    {
        private readonly CommandHandlerTemplate _template;
        private readonly IApplication _application;
        private readonly IMetadataManager _metadataManager;

        private readonly Lazy<StrategyData> _matchingElementDetails;

        public CreateImplementationStrategy(CommandHandlerTemplate template, IApplication application,
            IMetadataManager metadataManager)
        {
            _template = template;
            _application = application;
            _metadataManager = metadataManager;
            _matchingElementDetails = new Lazy<StrategyData>(GetMatchingElementDetails);
        }

        public bool IsMatch()
        {
            return _matchingElementDetails.Value.IsMatch;
        }

        public IEnumerable<RequiredService> GetRequiredServices()
        {
            return new[]
            {
                _matchingElementDetails.Value.Repository
            };
        }

        public string GetImplementation()
        {
            var foundEntity = _matchingElementDetails.Value.FoundEntity;
            var repository = _matchingElementDetails.Value.Repository;
            
            var entityName = _template.GetDomainEntityName(foundEntity);

            var codeLines = new List<string>();
            codeLines.Add(string.Empty);
            codeLines.Add($"var new{foundEntity.Name} = new {entityName ?? foundEntity.Name}");
            codeLines.Add($"{{");
            codeLines.AddRange(GetDTOPropertyAssignments("", "request", foundEntity, _template.Model.Properties).Select(s => $@"    {s}"));
            codeLines.Add($"}};");
            codeLines.Add(string.Empty);
            codeLines.Add($"{repository.FieldName}.Add(new{foundEntity.Name});");

            if (_template.Model.TypeReference.Element != null)
            {
                codeLines.Add(string.Empty);
                codeLines.Add($"await {repository.FieldName}.UnitOfWork.SaveChangesAsync(cancellationToken);");
                codeLines.Add($"return new{foundEntity.Name}.{foundEntity.Attributes.FirstOrDefault(x => x.HasPrimaryKey())?.Name.ToPascalCase() ?? "Id"};");
            }
            else
            {
                codeLines.Add(string.Empty);
                codeLines.Add($"return Unit.Value;");
            }
            
            const string newLine = @"
            ";
            return string.Join(newLine, codeLines);
        }

        private StrategyData GetMatchingElementDetails()
        {
            var commandNameLowercase = _template.Model.Name.ToLower();
            if ((commandNameLowercase.Contains("create") ||
                 commandNameLowercase.Contains("add") ||
                 commandNameLowercase.Contains("new"))
                && _template.Model.Mapping?.Element.IsClassModel() == true)
            {
                var foundEntity = _template.Model.Mapping.Element.AsClassModel();
                var repositoryInterface = _template.GetEntityRepositoryInterfaceName(foundEntity);
                if (repositoryInterface == null)
                {
                    return NoMatch;
                }

                var repository = new RequiredService(type: repositoryInterface,
                    name: repositoryInterface.Substring(1).ToCamelCase());

                return new StrategyData(true, foundEntity, repository);
            }

            var matchingEntities = _metadataManager.Domain(_application)
                .GetClassModels().Where(x => new[]
                {
                    $"add{x.Name.ToLower()}",
                    $"addnew{x.Name.ToLower()}",
                    $"create{x.Name.ToLower()}",
                    $"createnew{x.Name.ToLower()}",
                }.Contains(_template.Model.Name.ToLower().RemoveSuffix("command")))
                .ToList();

            if (matchingEntities.Count == 1)
            {
                var foundEntity = matchingEntities.Single();
                var repositoryInterface = _template.GetEntityRepositoryInterfaceName(foundEntity);
                if (repositoryInterface == null)
                {
                    return NoMatch;
                }

                var repository = new RequiredService(type: repositoryInterface,
                    name: repositoryInterface.Substring(1).ToCamelCase());
                
                return new StrategyData(true, foundEntity, repository);
            }

            return NoMatch;
        }

        public void OnStrategySelected()
        {
            var stack = new Stack<DTOModel>();
            var elementsFound = new List<(DTOModel Dto, string fieldName, ClassModel Domain)>();
            foreach (var commandProperty in _template.Model.Properties
                         .Where(p => p.Mapping?.Element?.SpecializationTypeId == AssociationTargetEndModel.SpecializationTypeId))
            {
                var association = commandProperty.Mapping.Element.AsAssociationTargetEndModel();
                var attributeClass = association.Class;
                var dto = commandProperty.TypeReference.Element.AsDTOModel();
                elementsFound.Add((dto, commandProperty.Name, attributeClass));
                stack.Push(dto);
            }

            while (stack.Any())
            {
                var dto = stack.Pop();
                foreach (var dtoField in dto.Fields
                             .Where(p => p.Mapping?.Element?.SpecializationTypeId == AssociationTargetEndModel.SpecializationTypeId))
                {
                    var association = dtoField.Mapping.Element.AsAssociationTargetEndModel();
                    var attributeClass = association.Class;
                    var nestedDto = dtoField.TypeReference.Element.AsDTOModel();
                    elementsFound.Add((nestedDto, dtoField.Name, attributeClass));
                    stack.Push(nestedDto);
                }
            }

            var @class = _template.CSharpFile.Classes.First();
            foreach (var match in elementsFound)
            {
                @class.AddMethod(_template.GetTypeName(match.Domain.InternalElement),
                    GetCreateMethodName(match.Domain),
                    method => method.Private()
                        .Static()
                        .AddAttribute("IntentManaged(Mode.Fully)")
                        .AddParameter(_template.GetTypeName(match.Dto.InternalElement), "dto")
                        .AddStatements(new[]
                        {
                            $"return new {match.Domain.Name.ToPascalCase()}",
                            "{",
                        })
                        .AddStatements(GetDTOPropertyAssignments("", $"dto", match.Domain, match.Dto.Fields))
                        .AddStatement("};"));
            }
        }
        
        private string GetCreateMethodName(ClassModel classModel)
        {
            return $"Create{classModel.Name.ToPascalCase()}";
        }

        private List<string> GetDTOPropertyAssignments(string entityVarName, string dtoVarName, ClassModel domainModel, IList<DTOFieldModel> dtoFields)
        {
            var codeLines = new List<string>();
            foreach (var field in dtoFields)
            {
                if (field.Mapping?.Element == null
                    && domainModel.Attributes.All(p => p.Name != field.Name))
                {
                    codeLines.Add($"#warning No matching field found for {field.Name}");
                    continue;
                }

                const string newLine = @"
    ";

                var entityVarExpr = !string.IsNullOrWhiteSpace(entityVarName) ? $"{entityVarName}." : string.Empty;
                switch (field.Mapping?.Element?.SpecializationTypeId)
                {
                    default:
                        var mappedPropertyName = field.Mapping?.Element?.Name ?? "<null>";
                        codeLines.Add($"#warning No matching type for Domain: {mappedPropertyName} and DTO: {field.Name}");
                        break;
                    case null:
                    case AttributeModel.SpecializationTypeId:
                        var attribute = field.Mapping?.Element?.AsAttributeModel()
                                        ?? domainModel.Attributes.First(p => p.Name == field.Name);
                        if (!attribute.Name.Equals("Id", StringComparison.OrdinalIgnoreCase))
                        {
                            codeLines.Add($"{entityVarExpr}{attribute.Name.ToPascalCase()} = {dtoVarName}.{field.Name.ToPascalCase()},");
                            break;
                        }
                        
                        break;
                    case AssociationTargetEndModel.SpecializationTypeId:
                    {
                        var association = field.Mapping.Element.AsAssociationTargetEndModel();
                        var attributeClass = association.Class;
                        var attributeName = association.Name.ToPascalCase();
                        
                        if (association.Association.AssociationType == AssociationType.Aggregation)
                        {
                            codeLines.Add($@"#warning Field not a composite association: {field.Name.ToPascalCase()}");
                            break;
                        }
                        
                        if (association.Multiplicity is Multiplicity.One or Multiplicity.ZeroToOne)
                        {
                            if (association.IsNullable)
                            {
                                codeLines.Add($"{entityVarExpr}{attributeName} = {dtoVarName}.{field.Name.ToPascalCase()} != null");
                                codeLines.Add($"    ? {GetCreateMethodName(attributeClass)}({dtoVarName}.{field.Name.ToPascalCase()})");
                                codeLines.Add($"    : null,");
                            }
                            else
                            {
                                codeLines.Add($"{entityVarExpr}{attributeName} = {GetCreateMethodName(attributeClass)}({dtoVarName}.{field.Name.ToPascalCase()}),");
                            }
                        }
                        else
                        {
                            codeLines.Add($"{entityVarExpr}{attributeName} = {dtoVarName}.{field.Name.ToPascalCase()}{(association.IsNullable?"?":"")}.Select({GetCreateMethodName(attributeClass)}).ToList(),");
                        }
                    }
                        break;
                }
            }

            return codeLines;
        }

        private static readonly StrategyData NoMatch = new StrategyData(false, null, null);
        private class StrategyData
        {
            public StrategyData(bool isMatch, ClassModel foundEntity, RequiredService repository)
            {
                IsMatch = isMatch;
                FoundEntity = foundEntity;
                Repository = repository;
            }
            
            public bool IsMatch { get; }
            public ClassModel FoundEntity { get; }
            public RequiredService Repository { get; }
        }
    }
}