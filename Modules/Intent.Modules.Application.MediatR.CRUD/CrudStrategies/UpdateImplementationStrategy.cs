using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Threading;
using Intent.Metadata.Models;
using Intent.Modelers.Domain.Api;
using Intent.Modelers.Services.Api;
using Intent.Modelers.Services.CQRS.Api;
using Intent.Modules.Application.MediatR.CRUD.Decorators;
using Intent.Modules.Application.MediatR.Templates;
using Intent.Modules.Application.MediatR.Templates.CommandHandler;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.CSharp.VisualStudio;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;
using Intent.Modules.Entities.Settings;
using Intent.Modules.Modelers.Domain.Settings;
using Intent.Templates;
using OperationModelExtensions = Intent.Modelers.Domain.Api.OperationModelExtensions;
using ParameterModelExtensions = Intent.Modelers.Domain.Api.ParameterModelExtensions;

namespace Intent.Modules.Application.MediatR.CRUD.CrudStrategies
{
    public class UpdateImplementationStrategy : ICrudImplementationStrategy
    {
        private readonly CSharpTemplateBase<CommandModel> _template;
        private readonly Lazy<StrategyData> _matchingElementDetails;

        public UpdateImplementationStrategy(CSharpTemplateBase<CommandModel> template)
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
            var @class = ((ICSharpFileBuilderTemplate)_template).CSharpFile.Classes.First(x => x.HasMetadata("handler"));
            _template.AddTypeSource(TemplateRoles.Domain.Entity.Primary);
            _template.AddTypeSource(TemplateRoles.Domain.ValueObject);
            _template.AddTypeSource(TemplateRoles.Domain.DataContract);
            _template.AddUsing("System.Linq");
            
            var ctor = @class.Constructors.First();
            var repository = _matchingElementDetails.Value.Repository;
            ctor.AddParameter(repository.Type, repository.Name.ToParameterName(), param => param.IntroduceReadonlyField());

            var handleMethod = @class.FindMethod("Handle");
            handleMethod.Statements.Clear();
            handleMethod.Attributes.OfType<CSharpIntentManagedAttribute>().SingleOrDefault()?.WithBodyFully();
            handleMethod.AddStatements(GetImplementation());
            if (_matchingElementDetails.Value.DtoToReturn != null)
            {
                ctor.AddParameter(_template.UseType("AutoMapper.IMapper"), "mapper", param => param.IntroduceReadonlyField());
            }
        }

        public IEnumerable<CSharpStatement> GetImplementation()
        {
            var foundEntity = _matchingElementDetails.Value.FoundEntity;
            var repository = _matchingElementDetails.Value.Repository;
            var idFields = _matchingElementDetails.Value.IdFields;
            var entityVariableName = foundEntity.GetExistingVariableName();

            var codeLines = new List<CSharpStatement>();
            var nestedCompOwner = _matchingElementDetails.Value.FoundEntity.GetNestedCompositionalOwner();
            if (nestedCompOwner != null)
            {
                var aggregateRootIdFields = _template.Model.Properties.GetNestedCompositionalOwnerIdFields(nestedCompOwner);
                if (!aggregateRootIdFields.Any())
                {
                    throw new Exception($"Nested Compositional Entity {foundEntity.Name} doesn't have an Id that refers to its owning Entity {nestedCompOwner.Name}.");
                }

                codeLines.Add($"var aggregateRoot = await {repository.FieldName}.FindByIdAsync({aggregateRootIdFields.GetEntityIdFromRequest(_template.Model.InternalElement)}, cancellationToken);");
                codeLines.Add(_template.CreateThrowNotFoundIfNullStatement(
                    variable: "aggregateRoot",
                    message: $"{{nameof({_template.GetTypeName(TemplateRoles.Domain.Entity.Primary, nestedCompOwner)})}} of Id '{aggregateRootIdFields.GetEntityIdFromRequestDescription()}' could not be found"));
                codeLines.Add(string.Empty);

                var association = nestedCompOwner.GetNestedCompositeAssociation(_matchingElementDetails.Value.FoundEntity);

                codeLines.Add($@"var {entityVariableName} = aggregateRoot.{association.Name.ToCSharpIdentifier(CapitalizationBehaviour.AsIs)}.FirstOrDefault({idFields.GetPropertyToRequestMatchClause()});");
                codeLines.Add(_template.CreateThrowNotFoundIfNullStatement(
                    variable: entityVariableName,
                    message: $"{{nameof({_template.GetTypeName(TemplateRoles.Domain.Entity.Primary, foundEntity)})}} of Id '{idFields.GetEntityIdFromRequestDescription()}' could not be found associated with {{nameof({_template.GetTypeName(TemplateRoles.Domain.Entity.Primary, nestedCompOwner)})}} of Id '{aggregateRootIdFields.GetEntityIdFromRequestDescription()}'"));
                codeLines.Add(string.Empty);

                codeLines.AddRange(GetDtoPropertyAssignments(entityVarName: entityVariableName, dtoVarName: "request", domainAttributes: foundEntity.Attributes, dtoFields: _template.Model.Properties.Where(FilterForAnaemicMapping).ToList(), skipIdField: true));

                if (RepositoryRequiresExplicitUpdate())
                {
                    codeLines.Add(new CSharpStatement($"{repository.FieldName}.Update(aggregateRoot);").SeparatedFromPrevious());
                }

                return codeLines;
            }

            codeLines.Add($"var {entityVariableName} = await {repository.FieldName}.FindByIdAsync({idFields.GetEntityIdFromRequest(_template.Model.InternalElement)}, cancellationToken);");
            codeLines.Add(_template.CreateThrowNotFoundIfNullStatement(
                variable: entityVariableName,
                message: $"Could not find {foundEntity.Name.ToPascalCase()} '{idFields.GetEntityIdFromRequestDescription()}'"));
            codeLines.Add(string.Empty);


            codeLines.AddRange(GetDtoPropertyAssignments(entityVarName: entityVariableName, dtoVarName: "request", domainAttributes: foundEntity.Attributes, dtoFields: _template.Model.Properties, skipIdField: true));

            if (RepositoryRequiresExplicitUpdate())
            {
                codeLines.Add(new CSharpStatement($"{repository.FieldName}.Update({entityVariableName});").SeparatedFromPrevious());
            }

            var dtoToReturn = _matchingElementDetails.Value.DtoToReturn;
            if (dtoToReturn != null)
            {
                codeLines.Add($"await {repository.FieldName}.UnitOfWork.SaveChangesAsync(cancellationToken);");
                codeLines.Add($@"return {entityVariableName}.MapTo{_template.GetDtoName(dtoToReturn)}(_mapper);");
            }

            return codeLines;

            bool FilterForAnaemicMapping(DTOFieldModel field)
            {
                return field.Mapping?.Element == null ||
                       field.Mapping.Element.IsAttributeModel() ||
                       field.Mapping.Element.IsAssociationEndModel();
            }
        }

        private bool RepositoryRequiresExplicitUpdate()
        {
            return _template.TryGetTemplate<ICSharpFileBuilderTemplate>(
                       TemplateRoles.Repository.Interface.Entity,
                       _matchingElementDetails.Value.RepositoryInterfaceModel,
                       out var repositoryInterfaceTemplate) &&
                   repositoryInterfaceTemplate.CSharpFile.Interfaces[0].TryGetMetadata<bool>("requires-explicit-update", out var requiresUpdate) &&
                   requiresUpdate;
        }

        private StrategyData GetMatchingElementDetails()
        {
            if (_template.ExecutionContext.Settings.GetDomainSettings().EnsurePrivatePropertySetters())
            {
                return NoMatch;
            }

            var commandNameLowercase = _template.Model.Name.ToLower();
            if ((!commandNameLowercase.StartsWith("update") &&
                 !commandNameLowercase.StartsWith("edit"))
                || _template.Model.Mapping?.Element.IsClassModel() != true)
            {
                return NoMatch;
            }

            var foundEntity = _template.Model.Mapping.Element.AsClassModel();

            var idFields = _template.Model.Properties.GetEntityIdFields(foundEntity, _template.ExecutionContext);
            if (!idFields.Any())
            {
                return NoMatch;
            }

            var nestedCompOwner = foundEntity.GetNestedCompositionalOwner();
            var repositoryInterfaceModel = nestedCompOwner != null ? nestedCompOwner : foundEntity;

            if (!_template.TryGetTypeName(TemplateRoles.Repository.Interface.Entity, repositoryInterfaceModel, out var repositoryInterface))
            {
                return NoMatch;
            }

            var repository = new RequiredService(type: repositoryInterface,
                name: repositoryInterface.Substring(1).ToCamelCase());

            var dtoToReturn = _template.Model.TypeReference.Element?.AsDTOModel();

            return new StrategyData(true, foundEntity, idFields, repository, dtoToReturn, repositoryInterfaceModel);
        }

        private IList<CSharpStatement> GetDtoPropertyAssignments(string entityVarName, string dtoVarName, IList<AttributeModel> domainAttributes, IList<DTOFieldModel> dtoFields, bool skipIdField)
        {
            var codeLines = new CSharpStatementAggregator();
            foreach (var field in dtoFields)
            {
                if (skipIdField && field.Name.Equals("id", StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                if (field.Mapping?.Element == null
                    && domainAttributes.All(p => p.Name != field.Name))
                {
                    codeLines.Add($"#warning No matching field found for {field.Name}");
                    continue;
                }

                var entityVarExpr = !string.IsNullOrWhiteSpace(entityVarName) ? $"{entityVarName}." : string.Empty;
                var fieldIsNullable = field.TypeReference.IsNullable;

                switch (field.Mapping?.Element?.SpecializationTypeId)
                {
                    default:
                        var mappedPropertyName = field.Mapping?.Element?.Name ?? "<null>";
                        codeLines.Add($"#warning No matching type for Domain: {mappedPropertyName} and DTO: {field.Name}");
                        break;
                    case null:
                    case AttributeModel.SpecializationTypeId:
                        var attribute = field.Mapping?.Element?.AsAttributeModel()
                                        ?? domainAttributes.First(p => p.Name == field.Name);
                        if (attribute.TypeReference?.Element?.SpecializationType == "Value Object")
                        {
                            var property = $"{entityVarExpr}{attribute.Name.ToPascalCase()}";
                            var updateMethodName = $"Create{attribute.TypeReference.Element.Name.ToPascalCase()}";
                            if (attribute.TypeReference.IsCollection)
                            {
                                codeLines.Add($"{property} = {dtoVarName}.{field.Name.ToPascalCase()}.Select(x => {updateMethodName}(x)).ToList());");
                            }
                            else
                            {
                                codeLines.Add($"{property} = {updateMethodName}({dtoVarName}.{field.Name.ToPascalCase()});");
                            }
                            ((ICSharpFileBuilderTemplate)_template).AddValueObjectFactoryMethod(updateMethodName, (IElement)attribute.TypeReference.Element, field);
                        }
                        else
                        {
                            var toListExpression = field.TypeReference.IsCollection
                                ? fieldIsNullable ? "?.ToList()" : ".ToList()"
                                : string.Empty;
                            codeLines.Add($"{entityVarExpr}{attribute.Name.ToPascalCase()} = {dtoVarName}.{field.Name.ToPascalCase()}{toListExpression};");
                        }
                        break;
                    case AssociationTargetEndModel.SpecializationTypeId:
                        {
                            var association = field.Mapping.Element.AsAssociationTargetEndModel();
                            var attributeName = association.Name.ToPascalCase();
                            var property = $"{entityVarExpr}{attributeName}";
                            if (association.Element.SpecializationType == "Value Object")
                            {
                                var targetValueObject = (IElement)association.Element;
                                var factoryMethodName = $"Create{targetValueObject.Name.ToPascalCase()}";
                                if (association.Multiplicity is Multiplicity.One or Multiplicity.ZeroToOne)
                                {
                                    codeLines.Add($"{property} = {factoryMethodName}({dtoVarName}.{field.Name.ToPascalCase()});");
                                }
                                else
                                {
                                    codeLines.Add($"{property} = {dtoVarName}.{field.Name.ToPascalCase()}.Select(x => {factoryMethodName}(x)).ToList();");
                                }
                                ((ICSharpFileBuilderTemplate)_template).AddValueObjectFactoryMethod(factoryMethodName, targetValueObject, field);
                                break;
                            }

                            var targetEntity = association.Element.AsClassModel();

                            if (association.Association.AssociationType == AssociationType.Aggregation)
                            {
                                codeLines.Add($@"#warning Field not a composite association: {field.Name.ToPascalCase()}");
                                break;
                            }

                            var updateMethodName = $"CreateOrUpdate{targetEntity.Name.ToPascalCase()}";

                            if (association.Multiplicity is Multiplicity.One or Multiplicity.ZeroToOne)
                            {
                                codeLines.Add($"{property} = {updateMethodName}({property}, {dtoVarName}.{field.Name.ToPascalCase()});");
                            }
                            else
                            {
                                var targetDto = field.TypeReference.Element.AsDTOModel();
                                codeLines.Add($"{property} = {_template.GetTypeName("Domain.Common.UpdateHelper")}.CreateOrUpdateCollection({property}, {dtoVarName}.{field.Name.ToPascalCase()}, {DomainToDtoMatch(targetDto.Fields.GetEntityIdFields(targetEntity, _template.ExecutionContext))}, {updateMethodName});");
                            }
                            AddCreateOrUpdateMethod(updateMethodName, targetEntity.InternalElement, field);
                        }
                        break;
                }
            }

            return codeLines.ToList();
        }

        private string DomainToDtoMatch(List<DTOFieldModel> dtoFields)
        {
            return $"(e, d) => {string.Join(" && ", dtoFields.Select(dtoField => $"e.{(dtoField.Mapping != null ? dtoField.Mapping.Element.AsAttributeModel().Name.ToPascalCase() : "Id")} == d.{dtoField.Name.ToPascalCase()}"))}";
        }

        public void BindToTemplate(ICSharpFileBuilderTemplate template)
        {
            template.CSharpFile.AfterBuild(_ => ApplyStrategy());
        }

        private void AddCreateOrUpdateMethod(string updateMethodName, IElement domainElement, DTOFieldModel field)
        {
            var createEntityInterfaces = _template.ExecutionContext.Settings.GetDomainSettings().CreateEntityInterfaces();            
            var implementationName = _template.GetTypeName(TemplateRoles.Domain.Entity.EntityImplementation, domainElement);
            var interfaceName = createEntityInterfaces ? _template.GetTypeName(TemplateRoles.Domain.Entity.Interface, domainElement) : implementationName;
            string nullableChar = _template.OutputTarget.GetProject().NullableEnabled ? "?" : "";

            var fieldIsNullable = field.TypeReference.IsNullable;
            var nullable = fieldIsNullable ? "?" : string.Empty;

            var @class = ((ICSharpFileBuilderTemplate)_template).CSharpFile.Classes.First(x => x.HasMetadata("handler"));
            var existingMethod = @class.FindMethod(x => x.Name == updateMethodName &&
                                                        x.ReturnType == $"{interfaceName}{nullable}" &&
                                                        x.Parameters.FirstOrDefault()?.Type == $"{interfaceName}{nullable}" &&
                                                        x.Parameters.Skip(1).FirstOrDefault()?.Type == _template.GetTypeName((IElement)field.TypeReference.Element));
            if (existingMethod == null)
            {

                @class.AddMethod($"{interfaceName}{nullable}",
                    updateMethodName,
                    method =>
                    {

                        method.Private()
                            .Static()
                            .AddAttribute(CSharpIntentManagedAttribute.Fully())
                            .AddParameter($"{interfaceName}{nullableChar}", "entity")
                            .AddParameter($"{_template.GetTypeName((IElement)field.TypeReference.Element)}{nullable}", "dto");

                        if (fieldIsNullable)
                        {
                            method.AddIfStatement("dto == null", s => s
                                .AddStatement("return null;"));
                        }

                        method.AddStatement($"entity ??= new {implementationName}();", s => s.SeparatedFromPrevious())
                            .AddStatements(GetDtoPropertyAssignments(
                                entityVarName: "entity",
                                dtoVarName: "dto",
                                domainAttributes: domainElement.GetDomainAttibuteModels(),
                                dtoFields: field.TypeReference.Element.AsDTOModel().Fields,
                                skipIdField: true))
                            .AddStatement("return entity;", s => s.SeparatedFromPrevious());
                    });
            }
        }

        private static readonly StrategyData NoMatch = new StrategyData(false, null, null, null, null, null);

        internal class StrategyData
        {
            public StrategyData(bool isMatch, ClassModel foundEntity, List<DTOFieldModel> idFields, RequiredService repository, DTOModel dtoToReturn, ClassModel repositoryInterfaceModel)
            {
                IsMatch = isMatch;
                FoundEntity = foundEntity;
                IdFields = idFields;
                Repository = repository;
                DtoToReturn = dtoToReturn;
                RepositoryInterfaceModel = repositoryInterfaceModel;
            }

            public bool IsMatch { get; }
            public ClassModel FoundEntity { get; }
            public List<DTOFieldModel> IdFields { get; }
            public RequiredService Repository { get; }
            public DTOModel DtoToReturn { get; }
            public ClassModel RepositoryInterfaceModel { get; }
        }
    }
}