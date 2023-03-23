using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
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
using Intent.Modules.Entities.Repositories.Api.Templates;
using JetBrains.Annotations;

namespace Intent.Modules.Application.MediatR.CRUD.CrudStrategies
{
    public class UpdateImplementationStrategy : ICrudImplementationStrategy
    {
        private readonly CommandHandlerTemplate _template;
        private readonly Lazy<StrategyData> _matchingElementDetails;

        public UpdateImplementationStrategy(CommandHandlerTemplate template)
        {
            _template = template;
            _matchingElementDetails = new Lazy<StrategyData>(GetMatchingElementDetails);
        }

        internal StrategyData GetStrategyData() => _matchingElementDetails.Value;

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
            var foundEntity = _matchingElementDetails.Value.FoundEntity;
            var repository = _matchingElementDetails.Value.Repository;
            var idField = _matchingElementDetails.Value.IdField;

            var codeLines = new List<CSharpStatement>();
            var nestedCompOwner = _matchingElementDetails.Value.FoundEntity.GetNestedCompositionalOwner();
            if (nestedCompOwner != null)
            {
                var aggregateRootField = _template.Model.Properties.GetNestedCompositionalOwnerIdField(nestedCompOwner);
                if (aggregateRootField == null)
                {
                    throw new Exception($"Nested Compositional Entity {foundEntity.Name} doesn't have an Id that refers to its owning Entity {nestedCompOwner.Name}.");
                }

                codeLines.Add($"var aggregateRoot = await {repository.FieldName}.FindByIdAsync(request.{aggregateRootField.Name.ToCSharpIdentifier(CapitalizationBehaviour.AsIs)}, cancellationToken);");
                codeLines.Add($"if (aggregateRoot == null)");
                codeLines.Add(new CSharpStatementBlock()
                    .AddStatement($@"throw new InvalidOperationException($""{{nameof({_template.GetTypeName(TemplateFulfillingRoles.Domain.Entity.Primary, nestedCompOwner)})}} of Id '{{request.{aggregateRootField.Name.ToCSharpIdentifier(CapitalizationBehaviour.AsIs)}}}' could not be found"");"));

                var association = nestedCompOwner.GetNestedCompositeAssociation(_matchingElementDetails.Value.FoundEntity);

                codeLines.Add($@"var element = aggregateRoot.{association.Name.ToCSharpIdentifier(CapitalizationBehaviour.AsIs)}.FirstOrDefault(p => p.{_matchingElementDetails.Value.FoundEntity.GetEntityIdAttribute(_template.ExecutionContext).IdName} == request.{idField.Name.ToPascalCase()});");
                codeLines.Add($"if (element == null)");
                codeLines.Add(new CSharpStatementBlock()
                    .AddStatement($@"throw new InvalidOperationException($""{{nameof({_template.GetTypeName(TemplateFulfillingRoles.Domain.Entity.Primary, foundEntity)})}} of Id '{{request.{idField.Name.ToPascalCase()}}}' could not be found associated with {{nameof({_template.GetTypeName(TemplateFulfillingRoles.Domain.Entity.Primary, nestedCompOwner)})}} of Id '{{request.{aggregateRootField.Name.ToCSharpIdentifier(CapitalizationBehaviour.AsIs)}}}'"");"));
                codeLines.AddRange(GetDTOPropertyAssignments(entityVarName: "element", dtoVarName: "request", domainModel: foundEntity, dtoFields: _template.Model.Properties, skipIdField: true));

                codeLines.Add("return Unit.Value;");

                return codeLines;
            }

            codeLines.Add($"var existing{foundEntity.Name} = await {repository.FieldName}.FindByIdAsync(request.{idField.Name.ToPascalCase()}, cancellationToken);");
            codeLines.AddRange(GetDTOPropertyAssignments(entityVarName: $"existing{foundEntity.Name}", dtoVarName: "request", domainModel: foundEntity, dtoFields: _template.Model.Properties, skipIdField: true));
            codeLines.Add($"return Unit.Value;");

            return codeLines;
        }

        private StrategyData GetMatchingElementDetails()
        {
            var commandNameLowercase = _template.Model.Name.ToLower();
            if ((commandNameLowercase.StartsWith("update") ||
                 commandNameLowercase.StartsWith("edit"))
                && _template.Model.Mapping?.Element.IsClassModel() == true)
            {
                var foundEntity = _template.Model.Mapping.Element.AsClassModel();

                var idField = _template.Model.Properties.GetEntityIdField(foundEntity);
                if (idField == null)
                {
                    return NoMatch;
                }

                var nestedCompOwner = foundEntity.GetNestedCompositionalOwner();
                var repositoryInterface = _template.GetEntityRepositoryInterfaceName(nestedCompOwner != null ? nestedCompOwner : foundEntity);
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

        private IList<CSharpStatement> GetDTOPropertyAssignments(string entityVarName, string dtoVarName, ClassModel domainModel, IList<DTOFieldModel> dtoFields, bool skipIdField)
        {
            var codeLines = new CSharpStatementAggregator();
            foreach (var field in dtoFields)
            {
                if (skipIdField && field.Name.Equals("id", StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                if (field.Mapping?.Element == null
                    && domainModel.Attributes.All(p => p.Name != field.Name))
                {
                    codeLines.Add($"#warning No matching field found for {field.Name}");
                    continue;
                }

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
                        var toListExpression = field.TypeReference.IsCollection
                            ? field.TypeReference.IsNullable ? "?.ToList()" : ".ToList()"
                            : string.Empty;
                        codeLines.Add($"{entityVarExpr}{attribute.Name.ToPascalCase()} = {dtoVarName}.{field.Name.ToPascalCase()}{toListExpression};");
                        break;
                    case AssociationTargetEndModel.SpecializationTypeId:
                        {
                            var association = field.Mapping.Element.AsAssociationTargetEndModel();
                            var targetEntity = association.Element.AsClassModel();
                            var attributeName = association.Name.ToPascalCase();

                            if (association.Association.AssociationType == AssociationType.Aggregation)
                            {
                                codeLines.Add($@"#warning Field not a composite association: {field.Name.ToPascalCase()}");
                                break;
                            }

                            var property = $"{entityVarExpr}{attributeName}";
                            var updateMethodName = $"CreateOrUpdate{targetEntity.InternalElement.Name.ToPascalCase()}";

                            if (association.Multiplicity is Multiplicity.One or Multiplicity.ZeroToOne)
                            {
                                codeLines.Add($"{property} = {updateMethodName}({property}, {dtoVarName}.{field.Name.ToPascalCase()});");
                            }
                            else
                            {
                                var targetDto = field.TypeReference.Element.AsDTOModel();
                                codeLines.Add($"{property} = {_template.GetTypeName("Domain.Common.UpdateHelper")}.CreateOrUpdateCollection({property}, {dtoVarName}.{field.Name.ToPascalCase()}, (e, d) => e.{targetEntity.GetEntityIdAttribute(_template.ExecutionContext).IdName} == d.{targetDto.Fields.GetEntityIdField(targetEntity).Name.ToPascalCase()}, {updateMethodName});");
                            }

                            var entityTypeName = _template.GetTypeName(targetEntity.InternalElement);
                            var @class = _template.CSharpFile.Classes.First();
                            @class.AddMethod(entityTypeName,
                                updateMethodName,
                                method =>
                                {
                                    method.Private()
                                        .Static()
                                        .AddAttribute(CSharpIntentManagedAttribute.Fully())
                                        .AddParameter(entityTypeName, "entity")
                                        .AddParameter(_template.GetTypeName((IElement)field.TypeReference.Element),
                                            "dto")
                                        .AddStatementBlock("if (dto == null)", s => s
                                            .AddStatement("return null;")
                                        )
                                        .AddStatement($"entity ??= new {entityTypeName}();", s => s.SeparatedFromPrevious())
                                        .AddStatements(GetDTOPropertyAssignments(
                                            entityVarName: "entity",
                                            dtoVarName: "dto",
                                            domainModel: targetEntity,
                                            dtoFields: field.TypeReference.Element.AsDTOModel().Fields,
                                            skipIdField: true))
                                        .AddStatement("return entity;", s => s.SeparatedFromPrevious());
                                });
                        }
                        break;
                }
            }

            return codeLines.ToList();
        }

        private static readonly StrategyData NoMatch = new StrategyData(false, null, null, null);

        internal class StrategyData
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