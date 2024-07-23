using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Intent.Metadata.Models;
using Intent.Modelers.Domain.Api;
using Intent.Modelers.Services.Api;
using Intent.Modelers.Services.CQRS.Api;
using Intent.Modelers.Services.DomainInteractions.Api;
using Intent.Modules.Application.MediatR.CRUD.Decorators;
using Intent.Modules.Application.MediatR.Templates;
using Intent.Modules.Application.MediatR.Templates.CommandHandler;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Mapping;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;
using Intent.Modules.Entities.Settings;
using Intent.Modules.Modelers.Domain.Settings;
using Intent.Templates;

namespace Intent.Modules.Application.MediatR.CRUD.CrudStrategies
{
    public class CreateImplementationStrategy : ICrudImplementationStrategy
    {
        private readonly CSharpTemplateBase<CommandModel> _template;

        private readonly Lazy<StrategyData> _matchingElementDetails;

        public CreateImplementationStrategy(CSharpTemplateBase<CommandModel> template)
        {
            _template = template;
            _matchingElementDetails = new Lazy<StrategyData>(GetMatchingElementDetails);
        }

        public bool IsMatch()
        {
            return _matchingElementDetails.Value.IsMatch;
        }

        public void BindToTemplate(ICSharpFileBuilderTemplate template)
        {
            template.CSharpFile.AfterBuild(_ => ApplyStrategy());
        }

        internal StrategyData GetStrategyData() => _matchingElementDetails.Value;

        public void ApplyStrategy()
        {
            _template.AddTypeSource(TemplateRoles.Domain.Entity.Primary);
            _template.AddTypeSource(TemplateRoles.Domain.ValueObject);
            _template.AddUsing("System.Linq");

            var @class = ((ICSharpFileBuilderTemplate)_template).CSharpFile.Classes.First(x => x.HasMetadata("handler"));
            var ctor = @class.Constructors.First();
            var repository = _matchingElementDetails.Value.Repository;
            ctor.AddParameter(repository.Type, repository.Name.ToParameterName(),
                param => param.IntroduceReadonlyField());

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
            var entityVariableName = foundEntity.GetNewVariableName();

            var entityName = _template.GetDomainEntityName(foundEntity) ?? foundEntity.Name;

            var codeLines = new CSharpStatementAggregator();

            var nestedCompOwner = foundEntity.GetNestedCompositionalOwner();
            if (nestedCompOwner != null)
            {
                var nestedCompOwnerIdFields = _template.Model.Properties.GetNestedCompositionalOwnerIdFields(owner: nestedCompOwner);
                if (!nestedCompOwnerIdFields.Any())
                {
                    throw new Exception($"Nested Compositional Entity {foundEntity.Name} doesn't have an Id that refers to its owning Entity {nestedCompOwner.Name}.");
                }

                codeLines.Add($"var aggregateRoot = await {repository.FieldName}.FindByIdAsync({nestedCompOwnerIdFields.GetEntityIdFromRequest(_template.Model.InternalElement)}, cancellationToken);");
                codeLines.Add(_template.CreateThrowNotFoundIfNullStatement(
                    variable: "aggregateRoot",
                    message: $"{{nameof({_template.GetTypeName(TemplateRoles.Domain.Entity.Primary, nestedCompOwner)})}} of Id '{nestedCompOwnerIdFields.GetEntityIdFromRequestDescription()}' could not be found"));
                codeLines.Add(string.Empty);
            }

            var assignmentStatements = GetDTOPropertyAssignments(entityVarName: "", dtoVarName: "request", domainAttributes: foundEntity.Attributes,
                dtoFields: _template.Model.Properties.Where(FilterForAnaemicMapping).ToList(),
                skipIdField: true);
            codeLines.Add($"var {entityVariableName} = new {entityName}{(assignmentStatements.Any() ? "" : "();")}");
            if (assignmentStatements.Any())
            {
                codeLines.Add(new CSharpStatementBlock()
                    .AddStatements(assignmentStatements)
                    .WithSemicolon());
            }


            if (nestedCompOwner != null)
            {
                var association = nestedCompOwner.GetNestedCompositeAssociation(foundEntity);
                codeLines.Add($"aggregateRoot.{association.Name.ToCSharpIdentifier(CapitalizationBehaviour.AsIs)}.Add({entityVariableName});", x => x.SeparatedFromPrevious());

                if (RepositoryRequiresExplicitUpdate())
                {
                    codeLines.Add($"{repository.FieldName}.Update(aggregateRoot);");
                }
            }
            else
            {
                codeLines.Add($"{repository.FieldName}.Add({entityVariableName});", x => x.SeparatedFromPrevious());
            }

            if (_template.Model.TypeReference.Element != null)
            {
                codeLines.Add($"await {repository.FieldName}.UnitOfWork.SaveChangesAsync(cancellationToken);");
                var dtoToReturn = _matchingElementDetails.Value.DtoToReturn;
                codeLines.Add(dtoToReturn != null
                    ? $"return {entityVariableName}.MapTo{_template.GetDtoName(dtoToReturn)}(_mapper);"
                    : $"return {entityVariableName}.{foundEntity.Attributes.Concat(foundEntity.ParentClass?.Attributes ?? new List<AttributeModel>()).FirstOrDefault(x => x.IsPrimaryKey())?.Name.ToPascalCase() ?? "Id"};");
            }

            return codeLines.ToList();

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

        private IList<CSharpStatement> GetDTOPropertyAssignments(string entityVarName, string dtoVarName, IList<AttributeModel> domainAttributes, IList<DTOFieldModel> dtoFields, bool skipIdField)
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
                                codeLines.Add($"{property} = {dtoVarName}.{field.Name.ToPascalCase()}.Select(x => {updateMethodName}(x)).ToList()),");
                            }
                            else
                            {
                                codeLines.Add($"{property} = {updateMethodName}({dtoVarName}.{field.Name.ToPascalCase()}),");
                            }
                            ((ICSharpFileBuilderTemplate)_template).AddValueObjectFactoryMethod(updateMethodName, (IElement)attribute.TypeReference.Element, field);
                        }
                        else
                        {
                            var toListExpression = field.TypeReference.IsCollection
                                ? field.TypeReference.IsNullable ? "?.ToList()" : ".ToList()"
                                : string.Empty;
                            codeLines.Add($"{entityVarExpr}{attribute.Name.ToPascalCase()} = {dtoVarName}.{field.Name.ToPascalCase()}{toListExpression},");
                        }
                        break;
                    case AssociationTargetEndModel.SpecializationTypeId:
                        {
                            var association = field.Mapping.Element.AsAssociationTargetEndModel();
                            var attributeName = association.Name.ToPascalCase();
                            if (association.Element.SpecializationType == "Value Object")
                            {
                                var targetValueObject = (IElement)association.Element;
                                var property = $"{entityVarExpr}{attributeName}";
                                var updateMethodName = $"Create{targetValueObject.Name.ToPascalCase()}";
                                if (association.Multiplicity is Multiplicity.One or Multiplicity.ZeroToOne)
                                {
                                    codeLines.Add($"{property} = {updateMethodName}({dtoVarName}.{field.Name.ToPascalCase()}),");
                                }
                                else
                                {
                                    codeLines.Add($"{property} = {dtoVarName}.{field.Name.ToPascalCase()}.Select(x => {updateMethodName}(x)).ToList(),");
                                }
                                ((ICSharpFileBuilderTemplate)_template).AddValueObjectFactoryMethod(updateMethodName, targetValueObject, field);
                                break;
                            }
                            var targetEntity = association.Element.AsClassModel();
                            var createMethodName = GetCreateMethodName(targetEntity.InternalElement);

                            if (association.Association.AssociationType == AssociationType.Aggregation)
                            {
                                codeLines.Add($@"#warning Field not a composite association: {field.Name.ToPascalCase()}");
                                break;
                            }

                            if (association.Multiplicity is Multiplicity.One or Multiplicity.ZeroToOne)
                            {
                                if (field.TypeReference.IsNullable)
                                {
                                    codeLines.Add(
                                        $"{entityVarExpr}{attributeName} = {dtoVarName}.{field.Name.ToPascalCase()} != null ? {createMethodName}({dtoVarName}.{field.Name.ToPascalCase()}) : null,");
                                }
                                else
                                {
                                    codeLines.Add($"{entityVarExpr}{attributeName} = {createMethodName}({dtoVarName}.{field.Name.ToPascalCase()}),");
                                }
                            }
                            else
                            {
                                codeLines.Add($"{entityVarExpr}{attributeName} = {dtoVarName}.{field.Name.ToPascalCase()}{(field.TypeReference.IsNullable ? "?" : "")}.Select({createMethodName}).ToList(){(field.TypeReference.IsNullable ? $" ?? new List<{targetEntity.Name.ToPascalCase()}>()" : "")},");
                            }

                            var @class = ((ICSharpFileBuilderTemplate)_template).CSharpFile.Classes.First(x => x.HasMetadata("handler"));
                            var existingMethod = @class.FindMethod(x => x.Name == createMethodName &&
                                                                        x.ReturnType == _template.GetTypeName(targetEntity.InternalElement) &&
                                                                        x.Parameters.FirstOrDefault()?.Type == _template.GetTypeName((IElement)field.TypeReference.Element));
                            if (existingMethod == null)
                            {
                                @class.AddMethod(_template.GetTypeName(targetEntity.InternalElement),
                                    createMethodName,
                                    method => method.Private()
                                        .Static()
                                        .AddAttribute(CSharpIntentManagedAttribute.Fully())
                                        .AddParameter(_template.GetTypeName((IElement)field.TypeReference.Element), "dto")
                                        .AddStatement($"return new {targetEntity.Name.ToPascalCase()}")
                                        .AddStatement(new CSharpStatementBlock()
                                            .AddStatements(GetDTOPropertyAssignments(
                                                entityVarName: "",
                                                dtoVarName: $"dto",
                                                domainAttributes: targetEntity.InternalElement.GetDomainAttibuteModels(),
                                                dtoFields: field.TypeReference.Element.AsDTOModel().Fields,
                                                skipIdField: true))
                                            .WithSemicolon()));
                            }
                        }
                        break;
                }
            }

            return codeLines.ToList();
        }

        private StrategyData GetMatchingElementDetails()
        {
            if (_template.ExecutionContext.Settings.GetDomainSettings().EnsurePrivatePropertySetters())
            {
                return NoMatch;
            }

            var commandNameLowercase = _template.Model.Name.ToLower();
            if ((!commandNameLowercase.StartsWith("create") &&
                 !commandNameLowercase.StartsWith("add") &&
                 !commandNameLowercase.StartsWith("new"))
                || _template.Model.Mapping?.Element.IsClassModel() != true)
            {
                return NoMatch;
            }

            var foundEntity = _template.Model.Mapping.Element.AsClassModel();
            var nestedCompOwner = foundEntity.GetNestedCompositionalOwner();
            var repositoryInterfaceModel = nestedCompOwner != null ? nestedCompOwner : foundEntity;

            
            if (!_template.TryGetTypeName(TemplateRoles.Repository.Interface.Entity, repositoryInterfaceModel, out var repositoryInterface))
            {
                return NoMatch;
            }

            var repository = new RequiredService(type: repositoryInterface,
                name: repositoryInterface.Substring(1).ToCamelCase());

            var dtoToReturn = _template.Model.TypeReference.Element?.AsDTOModel();

            return new StrategyData(true, foundEntity, repository, dtoToReturn, repositoryInterfaceModel);

        }

        private static string GetCreateMethodName(ICanBeReferencedType classModel)
        {
            return $"Create{classModel.Name.ToPascalCase()}";
        }

        private static readonly StrategyData NoMatch = new StrategyData(false, null, null, null, null);

        internal class StrategyData
        {
            public StrategyData(bool isMatch, ClassModel foundEntity, RequiredService repository, DTOModel dtoToReturn, ClassModel repositoryInterfaceModel)
            {
                IsMatch = isMatch;
                FoundEntity = foundEntity;
                Repository = repository;
                DtoToReturn = dtoToReturn;
                RepositoryInterfaceModel = repositoryInterfaceModel;
            }

            public bool IsMatch { get; }
            public ClassModel FoundEntity { get; }
            public RequiredService Repository { get; }
            public DTOModel DtoToReturn { get; }
            public ClassModel RepositoryInterfaceModel { get; }
        }
    }
}