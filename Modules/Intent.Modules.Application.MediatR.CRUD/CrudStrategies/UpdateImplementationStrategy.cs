using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Metadata.RDBMS.Api;
using Intent.Modelers.Domain.Api;
using Intent.Modelers.Services.Api;
using Intent.Modelers.Services.CQRS.Api;
using Intent.Modules.Application.MediatR.CRUD.Decorators;
using Intent.Modules.Application.MediatR.Templates;
using Intent.Modules.Application.MediatR.Templates.CommandHandler;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;
using JetBrains.Annotations;

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

        public void ApplyStrategy()
        {
            var @class = _template.CSharpFile.Classes.First();
            _template.AddTypeSource(TemplateFulfillingRoles.Domain.Entity.Primary);
            _template.AddTypeSource(TemplateFulfillingRoles.Domain.ValueObject);
            _template.AddUsing("System.Linq");
            var ctor = @class.Constructors.First();
            var repository = _matchingElementDetails.Value.Repository;
            ctor.AddParameter(repository.Type, repository.Name.ToParameterName(), param => param.IntroduceReadonlyField());

            var handleMethod = @class.FindMethod("Handle");
            handleMethod.Statements.Clear();
            handleMethod.Attributes.OfType<CSharpIntentManagedAttribute>().SingleOrDefault()?.WithBodyFully();
            handleMethod.AddStatements(GetImplementation());
        }

        public IEnumerable<CSharpStatement> GetImplementation()
        {
            var foundEntity = _matchingElementDetails.Value.FoundEntity.InternalElement;
            var repository = _matchingElementDetails.Value.Repository;
            var idField = _matchingElementDetails.Value.IdField;

            var codeLines = new List<CSharpStatement>();
            var aggrRootOwner = _matchingElementDetails.Value.FoundEntity.GetAggregateRootOwner();
            if (aggrRootOwner != null)
            {
                var aggregateRootField = _template.Model.Properties.GetForeignKeyFieldForAggregateRoot(aggrRootOwner);
                if (aggregateRootField == null)
                {
                    throw new Exception($"Nested Compositional Entity {foundEntity.Name} doesn't have an Id that refers to its owning Entity {aggrRootOwner.Name}.");
                }

                codeLines.Add($"var aggregateRoot = await {repository.FieldName}.FindByIdAsync(request.{aggregateRootField.Name.ToCSharpIdentifier(CapitalizationBehaviour.AsIs)}, cancellationToken);");
                codeLines.Add($"if (aggregateRoot == null)");
                codeLines.Add(new CSharpStatementBlock()
                    .AddStatement($@"throw new InvalidOperationException($""{{nameof({_template.GetTypeName(TemplateFulfillingRoles.Domain.Entity.Primary, aggrRootOwner)})}} of Id '{{request.{aggregateRootField.Name.ToCSharpIdentifier(CapitalizationBehaviour.AsIs)}}}' could not be found"");"));

                var association = aggrRootOwner.GetNestedCompositeAssociation(_matchingElementDetails.Value.FoundEntity);
                
                codeLines.Add($@"existingAggregateRoot.{association.Name.ToCSharpIdentifier(CapitalizationBehaviour.AsIs)} = request.{aggregateRootField.Name.ToCSharpIdentifier(CapitalizationBehaviour.AsIs)} != null");
                codeLines.Add($@"? (existingAggregateRoot.{association.Name.ToCSharpIdentifier(CapitalizationBehaviour.AsIs)} ?? new CompositeSingleA()).UpdateObject(request, UpdateCompositeCompositeSingleA)");
                codeLines.Add($@": null;");

                codeLines.Add("return Unit.Value;");

                return codeLines;
            }

            codeLines.Add($"var existing{foundEntity.Name} = await {repository.FieldName}.FindByIdAsync(request.{idField.Name.ToPascalCase()}, cancellationToken);");
            codeLines.AddRange(GetDTOPropertyAssignments($"existing{foundEntity.Name}", "request", foundEntity, _template.Model.Properties, true));
            codeLines.Add($"return Unit.Value;");

            return codeLines;
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

                var aggrRootOwner = foundEntity.GetAggregateRootOwner();
                var repositoryInterface = _template.GetEntityRepositoryInterfaceName(aggrRootOwner != null ? aggrRootOwner : foundEntity);
                if (repositoryInterface == null)
                {
                    return NoMatch;
                }

                var repository = new RequiredService(type: repositoryInterface,
                    name: repositoryInterface.Substring(1).ToCamelCase());

                return new StrategyData(true, foundEntity, idField, repository);
            }

            //var matchingEntities = _metadataManager.Domain(_application)
            //    .GetClassModels().Where(x => new[]
            //    {
            //        $"update{x.Name.ToLower()}",
            //        $"update{x.Name.ToLower()}details",
            //        $"edit{x.Name.ToLower()}",
            //        $"edit{x.Name.ToLower()}details",
            //    }.Contains(_template.Model.Name.ToLower().RemoveSuffix("command"))).ToList();

            //if (matchingEntities.Count == 1)
            //{
            //    var foundEntity = matchingEntities.Single();

            //    var idField = _template.Model.Properties.FirstOrDefault(p =>
            //        string.Equals(p.Name, "id", StringComparison.InvariantCultureIgnoreCase) ||
            //        string.Equals(p.Name, $"{foundEntity.Name}Id", StringComparison.InvariantCultureIgnoreCase));
            //    if (idField == null)
            //    {
            //        return NoMatch;
            //    }

            //    var repositoryInterface = _template.GetEntityRepositoryInterfaceName(foundEntity);
            //    if (repositoryInterface == null)
            //    {
            //        return NoMatch;
            //    }

            //    var repository = new RequiredService(type: repositoryInterface,
            //        name: repositoryInterface.Substring(1).ToCamelCase());

            //    return new StrategyData(true, foundEntity, idField, repository);
            //}

            return NoMatch;
        }

        private IList<CSharpStatement> GetDTOPropertyAssignments(string entityVarName, string dtoVarName, IElement domainModel, IList<DTOFieldModel> dtoFields, bool skipIdField)
        {
            var codeLines = new CSharpStatementAggregator();
            foreach (var field in dtoFields)
            {
                if (skipIdField && field.Name.Equals("id", StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                if (field.Mapping?.Element == null
                    && domainModel.ChildElements.All(p => p.Name != field.Name))
                {
                    codeLines.Add($"#warning No matching field found for {field.Name}");
                    continue;
                }

                switch (field.Mapping?.Element?.SpecializationTypeId)
                {
                    default:
                        var mappedPropertyName = field.Mapping?.Element?.Name ?? "<null>";
                        codeLines.Add($"#warning No matching type for Domain: {mappedPropertyName} and DTO: {field.Name}");
                        break;
                    case null:
                    case AttributeModel.SpecializationTypeId:
                        var attribute = field.Mapping?.Element
                                        ?? domainModel.ChildElements.First(p => p.Name == field.Name);
                        codeLines.Add($"{entityVarName}.{attribute.Name.ToPascalCase()} = {dtoVarName}.{field.Name.ToPascalCase()};");
                        break;
                    case AssociationTargetEndModel.SpecializationTypeId:
                        {
                            var association = field.Mapping.Element.AsAssociationTargetEndModel();
                            var targetEntity = (IElement)association.Element;
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
                                    codeLines.Add($"{entityVarName}.{attributeName} = {dtoVarName}.{field.Name.ToPascalCase()} != null");
                                    codeLines.Add($"? ({entityVarName}.{attributeName} ?? new {targetEntity.Name.ToPascalCase()}()).UpdateObject({dtoVarName}.{field.Name.ToPascalCase()}, {GetUpdateMethodName(targetEntity, attributeName)})", s => s.Indent());
                                    codeLines.Add($": null;", s => s.Indent());
                                }
                                else
                                {
                                    _template.AddUsing(_template.GetTemplate<IClassProvider>("Domain.Common.UpdateHelper").Namespace);
                                    codeLines.Add($"{entityVarName}.{attributeName}.UpdateObject({dtoVarName}.{field.Name.ToPascalCase()}, {GetUpdateMethodName(targetEntity, attributeName)});");
                                }
                            }
                            else
                            {
                                _template.AddUsing(_template.GetTemplate<IClassProvider>("Domain.Common.UpdateHelper").Namespace);
                                codeLines.Add($"{entityVarName}.{attributeName}{(association.IsNullable ? "?" : "")}.UpdateCollection({dtoVarName}.{field.Name.ToPascalCase()}, (x, y) => x.Id == y.Id, {GetUpdateMethodName(targetEntity, attributeName)});");
                            }

                            var @class = _template.CSharpFile.Classes.First();
                            @class.AddMethod("void",
                                GetUpdateMethodName(targetEntity, attributeName),
                                method => method.Private()
                                    .Static()
                                    .AddAttribute("IntentManaged(Mode.Fully)")
                                    .AddParameter(_template.GetTypeName(targetEntity), "entity")
                                    .AddParameter(_template.GetTypeName((IElement)field.TypeReference.Element), "dto")
                                    .AddStatements(GetDTOPropertyAssignments("entity", "dto", targetEntity, ((IElement)field.TypeReference.Element).ChildElements.Where(x => x.IsDTOFieldModel()).Select(x => x.AsDTOFieldModel()).ToList(), true)));
                        }
                        break;
                }
            }

            return codeLines.ToList();
        }

        private string GetUpdateMethodName(IElement classModel, [CanBeNull] string attibuteName)
        {
            return $"Update{(!string.IsNullOrEmpty(attibuteName) ? attibuteName : string.Empty)}{classModel.Name.ToPascalCase()}";
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