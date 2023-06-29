using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Reflection.Metadata;
using System.Xml.Linq;
using Intent.Metadata.Models;
using Intent.Modelers.Domain.Api;
using Intent.Modelers.Services.Api;
using Intent.Modules.Application.MediatR.CRUD.Decorators;
using Intent.Modules.Application.MediatR.Templates;
using Intent.Modules.Application.MediatR.Templates.CommandHandler;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;
using Intent.Modules.Entities.Repositories.Api.Templates;
using Intent.Modules.Entities.Settings;
using Intent.Modules.Modelers.Domain.Settings;
using OperationModelExtensions = Intent.Modelers.Domain.Api.OperationModelExtensions;
using ParameterModelExtensions = Intent.Modelers.Domain.Api.ParameterModelExtensions;

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
            _template.AddTypeSource(TemplateFulfillingRoles.Domain.DataContract);
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
                codeLines.Add(new CSharpIfStatement($"aggregateRoot is null")
                    .AddStatement($@"throw new {_template.GetNotFoundExceptionName()}($""{{nameof({_template.GetTypeName(TemplateFulfillingRoles.Domain.Entity.Primary, nestedCompOwner)})}} of Id '{{request.{aggregateRootField.Name.ToCSharpIdentifier(CapitalizationBehaviour.AsIs)}}}' could not be found"");"));

                var association = nestedCompOwner.GetNestedCompositeAssociation(_matchingElementDetails.Value.FoundEntity);

                codeLines.Add($@"var existing{foundEntity.Name} = aggregateRoot.{association.Name.ToCSharpIdentifier(CapitalizationBehaviour.AsIs)}.FirstOrDefault(p => p.{_matchingElementDetails.Value.FoundEntity.GetEntityIdAttribute(_template.ExecutionContext).IdName} == request.{idField.Name.ToPascalCase()});");
                codeLines.Add(new CSharpIfStatement($"existing{foundEntity.Name} is null")
                    .AddStatement($@"throw new {_template.GetNotFoundExceptionName()}($""{{nameof({_template.GetTypeName(TemplateFulfillingRoles.Domain.Entity.Primary, foundEntity)})}} of Id '{{request.{idField.Name.ToPascalCase()}}}' could not be found associated with {{nameof({_template.GetTypeName(TemplateFulfillingRoles.Domain.Entity.Primary, nestedCompOwner)})}} of Id '{{request.{aggregateRootField.Name.ToCSharpIdentifier(CapitalizationBehaviour.AsIs)}}}'"");"));
                codeLines.AddRange(GetDtoPropertyAssignments(entityVarName: $"existing{foundEntity.Name}", dtoVarName: "request", domainAttributes: foundEntity.Attributes, dtoFields: _template.Model.Properties.Where(FilterForAnaemicMapping).ToList(), skipIdField: true));

                if (RepositoryRequiresExplicitUpdate())
                {
                    codeLines.Add(new CSharpStatement($"{repository.FieldName}.Update(aggregateRoot);").SeparatedFromPrevious());
                }

                codeLines.Add("return Unit.Value;");
                return codeLines;
            }

            codeLines.Add($"var existing{foundEntity.Name} = await {repository.FieldName}.FindByIdAsync(request.{idField.Name.ToPascalCase()}, cancellationToken);");
            codeLines.Add(new CSharpIfStatement($"existing{foundEntity.Name} is null")
                .AddStatement($@"throw new {_template.GetNotFoundExceptionName()}($""Could not find {foundEntity.Name.ToPascalCase()} {{request.{idField.Name.ToPascalCase()}}}"");"));
            codeLines.AddRange(GetDtoPropertyAssignments(entityVarName: $"existing{foundEntity.Name}", dtoVarName: "request", domainAttributes: foundEntity.Attributes, dtoFields: _template.Model.Properties, skipIdField: true));

            if (RepositoryRequiresExplicitUpdate())
            {
                codeLines.Add(new CSharpStatement($"{repository.FieldName}.Update(existing{foundEntity.Name});").SeparatedFromPrevious());
            }

            var dtoToReturn = _matchingElementDetails.Value.DtoToReturn;
            if (dtoToReturn != null)
            {
                codeLines.Add($"await {repository.FieldName}.UnitOfWork.SaveChangesAsync(cancellationToken);");
                codeLines.Add($@"return existing{foundEntity.Name}.MapTo{_template.GetDtoName(dtoToReturn)}(_mapper);");
            }
            else
            {
                codeLines.Add($"return Unit.Value;");
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
                       TemplateFulfillingRoles.Repository.Interface.Entity,
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

            var idField = _template.Model.Properties.GetEntityIdField(foundEntity);
            if (idField == null)
            {
                return NoMatch;
            }

            var nestedCompOwner = foundEntity.GetNestedCompositionalOwner();
            var repositoryInterfaceModel = nestedCompOwner != null ? nestedCompOwner : foundEntity;

            var repositoryInterface = _template.GetEntityRepositoryInterfaceName(repositoryInterfaceModel);
            if (repositoryInterface == null)
            {
                return NoMatch;
            }

            var repository = new RequiredService(type: repositoryInterface,
                name: repositoryInterface.Substring(1).ToCamelCase());

            var dtoToReturn = _template.Model.TypeReference.Element?.AsDTOModel();

            return new StrategyData(true, foundEntity, idField, repository, dtoToReturn, repositoryInterfaceModel);
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
                            AddValueObjectFactoryMethod(updateMethodName, (IElement)attribute.TypeReference.Element, field);
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
                                    codeLines.Add($"{property} = {dtoVarName}.{field.Name.ToPascalCase()}.Select(x => {factoryMethodName}(x)).ToList());");
                                }
                                AddValueObjectFactoryMethod(factoryMethodName, targetValueObject, field);
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
                                codeLines.Add($"{property} = {_template.GetTypeName("Domain.Common.UpdateHelper")}.CreateOrUpdateCollection({property}, {dtoVarName}.{field.Name.ToPascalCase()}, (e, d) => e.{targetEntity.GetEntityIdAttribute(_template.ExecutionContext).IdName} == d.{targetDto.Fields.GetEntityIdField(targetEntity).Name.ToPascalCase()}, {updateMethodName});");
                            }
                            AddCreateOrUpdateMethod(updateMethodName, targetEntity.InternalElement, field );
                        }
                        break;
                }
            }

            return codeLines.ToList();
        }

        private void AddValueObjectFactoryMethod(string mappingMethodName, IElement domain, DTOFieldModel field)
        {
            var @class = _template.CSharpFile.Classes.First();
            var targetDto = field.TypeReference.Element.AsDTOModel();
            if (!MethodExists(mappingMethodName, @class, targetDto))
            {
                var domainType = _template.GetTypeName(domain);
                @class.AddMethod(domainType, mappingMethodName, method =>
                {
                    method.Static()
                        .AddAttribute(CSharpIntentManagedAttribute.Fully())
                        .AddParameter(_template.GetTypeName(targetDto.InternalElement), "dto");

                    var attributeModels = GetDomainAttibuteModels(domain);

                    var attributeMap = attributeModels.Select(a => (Domain: a, Dto:  targetDto.Fields.FirstOrDefault(f => f.Mapping?.Element.Id == a.Id)));
                    if (attributeMap.Any(x => x.Dto == null))
                    {
                        method.AddStatement($@"#warning Not all fields specified for ValueObject.");
                    }
                    var ctorParameters = string.Join(",", attributeMap.Select(m => $"{m.Domain.Name.ToParameterName()}: {(m.Dto == null ? $"default({_template.GetTypeName(m.Domain.TypeReference)})" : $"dto.{m.Dto.Name.ToPascalCase()}")}"));
                    method.AddStatement($"return new {domainType}({ctorParameters});");
                });
            }
        }

        private bool MethodExists(string mappingMethodName, CSharpClass @class, DTOModel targetDto)
        {
            return @class.FindMethod((method) =>
                                        method.Name == mappingMethodName
                                        && method.Parameters.Count == 1
                                        && method.Parameters[0].Type == _template.GetTypeName(targetDto.InternalElement)) != null;
        }

        private void AddCreateOrUpdateMethod(string updateMethodName, IElement domainElement, DTOFieldModel field)
        {
            var domainTypeName = _template.GetTypeName(domainElement);
            var fieldIsNullable = field.TypeReference.IsNullable;

            var @class = _template.CSharpFile.Classes.First();
            var existingMethod = @class.FindMethod(x => x.Name == updateMethodName &&
                                                        x.ReturnType == domainTypeName &&
                                                        x.Parameters.FirstOrDefault()?.Type == domainTypeName &&
                                                        x.Parameters.Skip(1).FirstOrDefault()?.Type == _template.GetTypeName((IElement)field.TypeReference.Element));
            if (existingMethod == null)
            {
                var nullable = fieldIsNullable ? "?" : string.Empty;

                @class.AddMethod($"{domainTypeName}{nullable}",
                    updateMethodName,
                    method =>
                    {

                        method.Private()
                            .Static()
                            .AddAttribute(CSharpIntentManagedAttribute.Fully())
                            .AddParameter($"{domainTypeName}{nullable}", "entity")
                            .AddParameter($"{_template.GetTypeName((IElement)field.TypeReference.Element)}{nullable}", "dto");

                        if (fieldIsNullable)
                        {
                            method.AddIfStatement("dto == null", s => s
                                .AddStatement("return null;"));
                        }

                        method.AddStatement($"entity ??= new {domainTypeName}();", s => s.SeparatedFromPrevious())
                            .AddStatements(GetDtoPropertyAssignments(
                                entityVarName: "entity",
                                dtoVarName: "dto",
                                domainAttributes: GetDomainAttibuteModels(domainElement),
                                dtoFields: field.TypeReference.Element.AsDTOModel().Fields,
                                skipIdField: true))
                            .AddStatement("return entity;", s => s.SeparatedFromPrevious());
                    });
            }
        }

        private IList<AttributeModel> GetDomainAttibuteModels(IElement element)
        {
            return element.ChildElements.Where(x => x.IsAttributeModel()).Select(x => x.AsAttributeModel()).ToList();
        }

        private static readonly StrategyData NoMatch = new StrategyData(false, null, null, null, null, null);

        internal class StrategyData
        {
            public StrategyData(bool isMatch, ClassModel foundEntity, DTOFieldModel idField, RequiredService repository, DTOModel dtoToReturn, ClassModel repositoryInterfaceModel)
            {
                IsMatch = isMatch;
                FoundEntity = foundEntity;
                IdField = idField;
                Repository = repository;
                DtoToReturn = dtoToReturn;
                RepositoryInterfaceModel = repositoryInterfaceModel;
            }

            public bool IsMatch { get; }
            public ClassModel FoundEntity { get; }
            public DTOFieldModel IdField { get; }
            public RequiredService Repository { get; }
            public DTOModel DtoToReturn { get; }
            public ClassModel RepositoryInterfaceModel { get; }
        }
    }
}