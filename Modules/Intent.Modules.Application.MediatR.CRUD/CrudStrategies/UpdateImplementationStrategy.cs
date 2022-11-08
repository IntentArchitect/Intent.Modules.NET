using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.RDBMS.Api;
using Intent.Modelers.Domain.Api;
using Intent.Modelers.Services.Api;
using Intent.Modelers.Services.CQRS.Api;
using Intent.Modules.Application.MediatR.CRUD.Decorators;
using Intent.Modules.Application.MediatR.Templates;
using Intent.Modules.Application.MediatR.Templates.CommandHandler;
using Intent.Modules.Common.Templates;

namespace Intent.Modules.Application.MediatR.CRUD.CrudStrategies
{
    class UpdateImplementationStrategy : ICrudImplementationStrategy
    {
        private readonly CommandHandlerTemplate _template;
        private readonly IApplication _application;
        private readonly IMetadataManager _metadataManager;
        private readonly Lazy<StrategyData> _matchingElementDetails;

        public UpdateImplementationStrategy(CommandHandlerTemplate template, IApplication application,
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
            var idField = _matchingElementDetails.Value.IdField;

            var entityName = _template.GetDomainEntityName(foundEntity);
            
            var codeLines = new List<string>();
            codeLines.Add(string.Empty);
            codeLines.Add($"var existing{foundEntity.Name} = await {repository.FieldName}.FindByIdAsync(request.{idField.Name.ToPascalCase()}, cancellationToken);");
            codeLines.AddRange(GetCommandPropertyAssignments(foundEntity, _template.Model));
            codeLines.Add(string.Empty);
            codeLines.Add($"return Unit.Value;");
            
            const string newLine = @"
            ";
            return string.Join(newLine, codeLines);
        }

        private StrategyData GetMatchingElementDetails()
        {
            var commandNameLowercase = _template.Model.Name.ToLower();
            if ((commandNameLowercase.Contains("update") ||
                 commandNameLowercase.Contains("edit"))
                && _template.Model.Mapping?.Element.IsClassModel() == true)
            {
                var foundEntity = _template.Model.Mapping.Element.AsClassModel();
                
                var idField = _template.Model.Properties.FirstOrDefault(p =>
                    string.Equals(p.Name, "id", StringComparison.InvariantCultureIgnoreCase) ||
                    string.Equals(p.Name, $"{foundEntity.Name}Id", StringComparison.InvariantCultureIgnoreCase));
                if (idField == null)
                {
                    return NoMatch;
                }
                
                var repositoryInterface = _template.GetEntityRepositoryInterfaceName(foundEntity);
                if (repositoryInterface == null)
                {
                    return NoMatch;
                }

                var repository = new RequiredService(type: repositoryInterface,
                    name: repositoryInterface.Substring(1).ToCamelCase());

                return new StrategyData(true, foundEntity, idField, repository);
            }
            
            var matchingEntities = _metadataManager.Domain(_application)
                .GetClassModels().Where(x => new[]
                {
                    $"update{x.Name.ToLower()}",
                    $"update{x.Name.ToLower()}details",
                    $"edit{x.Name.ToLower()}",
                    $"edit{x.Name.ToLower()}details",
                }.Contains(_template.Model.Name.ToLower().RemoveSuffix("command"))).ToList();

            if (matchingEntities.Count == 1)
            {
                var foundEntity = matchingEntities.Single();

                var idField = _template.Model.Properties.FirstOrDefault(p =>
                    string.Equals(p.Name, "id", StringComparison.InvariantCultureIgnoreCase) ||
                    string.Equals(p.Name, $"{foundEntity.Name}Id", StringComparison.InvariantCultureIgnoreCase));
                if (idField == null)
                {
                    return NoMatch;
                }

                var repositoryInterface = _template.GetEntityRepositoryInterfaceName(foundEntity);
                if (repositoryInterface == null)
                {
                    return NoMatch;
                }

                var repository = new RequiredService(type: repositoryInterface,
                    name: repositoryInterface.Substring(1).ToCamelCase());
            
                return new StrategyData(true, foundEntity, idField, repository);
            }
            
            return NoMatch;
        }

        private List<string> GetCommandPropertyAssignments(ClassModel domainModel, CommandModel command)
        {
            var codeLines = new List<string>();
            foreach (var property in command.Properties)
            {
                if (property.Name.Equals("id", StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }
                
                if (property.Mapping?.Element == null
                    && domainModel.Attributes.All(p => p.Name != property.Name))
                {
                    codeLines.Add($"#warning No matching field found for {property.Name}");
                    continue;
                }

                const string newLine = @"
    ";

                switch (property.Mapping?.Element?.SpecializationTypeId)
                {
                    default:
                        var mappedPropertyName = property.Mapping?.Element?.Name ?? "<null>";
                        codeLines.Add($"#warning No matching type for Domain: {mappedPropertyName} and DTO: {property.Name}");
                        break;
                    case AttributeModel.SpecializationTypeId:
                        var attribute = property.Mapping?.Element?.AsAttributeModel()
                                        ?? domainModel.Attributes.First(p => p.Name == property.Name);
                        codeLines.Add($"existing{domainModel.Name}.{attribute.Name.ToPascalCase()} = request.{property.Name.ToPascalCase()};");
                        break;
                    case AssociationTargetEndModel.SpecializationTypeId:
                    {
                        var association = property.Mapping.Element.AsAssociationTargetEndModel();
                        var attributeClass = association.Class;
                        var attributeName = association.Name.ToPascalCase();
                        
                        if (association.Association.AssociationType == AssociationType.Aggregation)
                        {
                            codeLines.Add($@"#warning Property not a composite association: {property.Name.ToPascalCase()}");
                            break;
                        }
                        
                        if (association.Multiplicity is Multiplicity.One or Multiplicity.ZeroToOne)
                        {
                            if (association.IsNullable)
                            {
                                codeLines.Add($"existing{domainModel.Name}.{attributeName} = request.{property.Name.ToPascalCase()} != null");
                                codeLines.Add($"    ? new {attributeClass.Name.ToPascalCase()}");
                                codeLines.Add($"    {{");
                                codeLines.AddRange(GetDTOPropertyAssignments($"request.{property.Name.ToPascalCase()}", attributeClass, property.TypeReference.Element.AsDTOModel())
                                    .Select(s => $"        {s}"));
                                codeLines.Add($"    }}");
                                codeLines.Add($"    : null;");
                            }
                            else
                            {
                                codeLines.Add($"existing{domainModel.Name}.{attributeName} = new {attributeClass.Name.ToPascalCase()}");
                                codeLines.Add($"    {{");
                                codeLines.AddRange(GetDTOPropertyAssignments($"request.{property.Name.ToPascalCase()}", attributeClass, property.TypeReference.Element.AsDTOModel())
                                    .Select(s => $"        {s}"));
                                codeLines.Add($"    }};");
                            }
                        }
                        else
                        {
                            codeLines.Add($"existing{domainModel.Name}.{attributeName} = request.{property.Name.ToPascalCase()}{(association.IsNullable?"?":"")}.Select({property.Name.ToCamelCase()} =>");
                            codeLines.Add($"    new {attributeClass.Name.ToPascalCase()}");
                            codeLines.Add($"    {{");
                            codeLines.AddRange(GetDTOPropertyAssignments(property.Name.ToCamelCase(), attributeClass, property.TypeReference.Element.AsDTOModel())
                                .Select(s => $"        {s}"));
                            codeLines.Add($"    }}).ToList();");
                        }
                    }
                        break;
                }
            }

            return codeLines;
        }

        private List<string> GetDTOPropertyAssignments(string accessorName, ClassModel domainModel, DTOModel dto)
        {
            var codeLines = new List<string>();
            foreach (var field in dto.Fields)
            {
                if (field.Mapping?.Element == null
                    && domainModel.Attributes.All(p => p.Name != field.Name))
                {
                    codeLines.Add($"#warning No matching field found for {field.Name}");
                    continue;
                }

                const string newLine = @"
    ";

                switch (field.Mapping?.Element?.SpecializationTypeId)
                {
                    default:
                        var mappedPropertyName = field.Mapping?.Element?.Name ?? "<null>";
                        codeLines.Add($"#warning No matching type for Domain: {mappedPropertyName} and DTO: {field.Name}");
                        break;
                    case AttributeModel.SpecializationTypeId:
                        var attribute = field.Mapping?.Element?.AsAttributeModel()
                                        ?? domainModel.Attributes.First(p => p.Name == field.Name);
                        codeLines.Add($"{attribute.Name.ToPascalCase()} = {accessorName}.{field.Name.ToPascalCase()},");
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
                                codeLines.Add($"{attributeName} = {accessorName}.{field.Name.ToPascalCase()} != null");
                                codeLines.Add($"    ? new {attributeClass.Name.ToPascalCase()}");
                                codeLines.Add($"    {{");
                                codeLines.AddRange(GetDTOPropertyAssignments($"{accessorName}.{field.Name.ToPascalCase()}", attributeClass,
                                        field.TypeReference.Element.AsDTOModel())
                                    .Select(s => $"        {s}"));
                                codeLines.Add($"    }}");
                                codeLines.Add($"    : null,");
                            }
                            else
                            {
                                codeLines.Add($"{attributeName} = new {attributeClass.Name.ToPascalCase()}");
                                codeLines.Add($"    {{");
                                codeLines.AddRange(GetDTOPropertyAssignments($"{accessorName}.{field.Name.ToPascalCase()}", attributeClass, field.TypeReference.Element.AsDTOModel())
                                    .Select(s => $"        {s}"));
                                codeLines.Add($"    }},");
                            }
                        }
                        else
                        {
                            codeLines.Add($"{attributeName} = {accessorName}.{field.Name.ToPascalCase()}{(association.IsNullable?"?":"")}.Select({field.Name.ToCamelCase()} =>");
                            codeLines.Add($"    new {attributeClass.Name.ToPascalCase()}");
                            codeLines.Add($"    {{");
                            codeLines.AddRange(GetDTOPropertyAssignments(field.Name.ToCamelCase(), attributeClass, field.TypeReference.Element.AsDTOModel())
                                .Select(s => $"        {s}"));
                            codeLines.Add($"    }}).ToList(),");
                        }
                    }
                        break;
                }
            }

            return codeLines;
        }
        
        private static readonly StrategyData NoMatch = new StrategyData(false, null, null, null);

        private class StrategyData
        {
            public StrategyData(bool isMatch, ClassModel foundEntity, DTOFieldModel idField, RequiredService repository)
            {
                IsMatch = isMatch;
                FoundEntity = foundEntity;
                IdField = idField;
                Repository = repository;
            }

            public bool IsMatch { get; }
            public ClassModel FoundEntity { get; }
            public DTOFieldModel IdField { get; }
            public RequiredService Repository { get; }
        }
    }
}