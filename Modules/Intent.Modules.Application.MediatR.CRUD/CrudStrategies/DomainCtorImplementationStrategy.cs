using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Exceptions;
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
using ParameterModel = Intent.Modelers.Domain.Api.ParameterModel;

namespace Intent.Modules.Application.MediatR.CRUD.CrudStrategies
{
    public class DomainCtorImplementationStrategy : ICrudImplementationStrategy
    {
        private readonly CommandHandlerTemplate _template;

        private readonly Lazy<StrategyData> _matchingElementDetails;

        public DomainCtorImplementationStrategy(CommandHandlerTemplate template)
        {
            _template = template;
            _matchingElementDetails = new Lazy<StrategyData>(GetMatchingElementDetails);
        }

        public bool IsMatch()
        {
            return _matchingElementDetails.Value.IsMatch;
        }

        internal StrategyData GetStrategyData() => _matchingElementDetails.Value;

        public void ApplyStrategy()
        {
            _template.AddTypeSource(TemplateFulfillingRoles.Domain.Entity.Primary);
            _template.AddTypeSource(TemplateFulfillingRoles.Domain.ValueObject);
            _template.AddTypeSource(TemplateFulfillingRoles.Domain.DataContract);
            _template.AddUsing("System.Linq");

            var @class = _template.CSharpFile.Classes.First();
            var ctor = @class.Constructors.First();
            var repository = _matchingElementDetails.Value.Repository;
            ctor.AddParameter(repository.Type, repository.Name.ToParameterName(),
                param => param.IntroduceReadonlyField());

            foreach (var requiredService in _matchingElementDetails.Value.AdditionalServices)
            {
                ctor.AddParameter(requiredService.Type, requiredService.Name, c => c.IntroduceReadonlyField());
            }

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
                var nestedCompOwnerIdFields = _template.Model.Properties.GetNestedCompositionalOwnerIdFields(nestedCompOwner, foundEntity);
                if (!nestedCompOwnerIdFields.Any())
                {
                    throw new Exception($"Nested Compositional Entity {foundEntity.Name} doesn't have an Id that refers to its owning Entity {nestedCompOwner.Name}.");
                }

                codeLines.Add($"var aggregateRoot = await {repository.FieldName}.FindByIdAsync({nestedCompOwnerIdFields.GetEntityIdFromRequest()}, cancellationToken);");
                codeLines.Add(_template.CreateThrowNotFoundIfNullStatement(
                    variable: "aggregateRoot",
                    message: $"{{nameof({_template.GetTypeName(TemplateFulfillingRoles.Domain.Entity.Primary, nestedCompOwner)})}} of Id '{nestedCompOwnerIdFields.GetEntityIdFromRequestDescription()}' could not be found"));
                codeLines.Add(string.Empty);
            }

            codeLines.Add(GetConstructorStatement(entityVariableName, "request", false));

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

            if (_template.Model.TypeReference.Element != null ||
                RepositoryRequiresExplicitUpdate())
            {
                codeLines.Add($"await {repository.FieldName}.UnitOfWork.SaveChangesAsync(cancellationToken);");
            }

            if (_template.Model.TypeReference.Element != null)
            {
                var dtoToReturn = _matchingElementDetails.Value.DtoToReturn;
                codeLines.Add(dtoToReturn != null
                    ? $"return {entityVariableName}.MapTo{_template.GetDtoName(dtoToReturn)}(_mapper);"
                    : $"return {entityVariableName}.{foundEntity.Attributes.Concat(foundEntity.ParentClass?.Attributes ?? new List<AttributeModel>()).FirstOrDefault(x => x.IsPrimaryKey())?.Name.ToPascalCase() ?? "Id"};");
            }

            return codeLines.ToList();

            CSharpStatement GetConstructorStatement(string entityVarName, string dtoVarName, bool hasInitStatements)
            {

                var ctor = _template.Model.Mapping.Element.AsClassConstructorModel();
                var ctorParams = ctor?.Parameters;

                if (ctorParams?.Any() != true)
                {
                    return $"var {entityVarName} = new {entityName}{(hasInitStatements ? "" : "();")}";
                }

                var invocationStatement = new CSharpInvocationStatement($"var {entityVarName} = new {entityName}");
                if (hasInitStatements)
                {
                    invocationStatement.WithoutSemicolon();
                }

                try
                {
                    foreach (var param in ctorParams)
                    {
                        var invocationArgument = GetInvocationArgument(param, _template.Model.Properties, dtoVarName);
                        if (invocationArgument == null)
                        {
                            codeLines.Add($"#warning No supported convention for populating \"{param.Name.ToParameterName()}\" parameter");
                            invocationStatement.AddArgument($"{param.Name.ToParameterName()}: default");
                            continue;
                        }

                        invocationStatement.AddArgument(invocationArgument);
                    }

                    return invocationStatement;
                }
                catch (Exception ex)
                {
                    throw new ElementException(_template.Model.InternalElement, $"Constructor mapping to [{ctor.ParentClass.Name}] could not be performed. See inner exception for more details.", ex);
                }
            }
        }

        private CSharpStatement GetInvocationArgument(
            ParameterModel parameter,
            IEnumerable<DTOFieldModel> fields,
            string dtoVarName)
        {
            if (parameter.TypeReference?.Element?.SpecializationType is "Value Object" or "Data Contract")
            {
                var mappingMethodName = $"Create{parameter.TypeReference.Element.Name.ToPascalCase()}";

                var mappedField = fields.FirstOrDefault(field => field.Mapping?.Element?.Id == parameter.Id);
                if (mappedField == null)
                {
                    throw new Exception($"A mapping doesn't exist for parameter '{parameter.Name}'");
                }
                var constructMethod = parameter.TypeReference.IsCollection
                    ? $"{dtoVarName}.{mappedField.Name.ToPascalCase()}.Select({mappingMethodName})"
                    : $"{mappingMethodName}({dtoVarName}.{mappedField.Name.ToPascalCase()})";
                AddMappingMethod(mappingMethodName, mappedField, (IElement)parameter.TypeReference.Element);
                return constructMethod;
            }

            var dtoFieldRef = fields.Where(field => field.Mapping?.Element?.Id == parameter.Id)
                .Select(field => $"{dtoVarName}.{field.Name.ToPascalCase()}")
                .FirstOrDefault();
            if (dtoFieldRef != null)
            {
                return dtoFieldRef;
            }

            var service = _template.FindRequiredService(parameter.Type.Element);
            if (service != null)
            {
                return service.FieldName;
            }

            return null;
        }

        private void AddMappingMethod(string mappingMethodName, DTOFieldModel field, IElement element)
        {
            var @class = _template.CSharpFile.Classes.First();
            var targetDto = field.TypeReference.Element.AsDTOModel();
            if (!MethodExists(mappingMethodName, @class, targetDto))
            {
                var domainType = _template.GetTypeName(element);
                @class.AddMethod(domainType, mappingMethodName, method =>
                {
                    method.Static()
                        .AddAttribute(CSharpIntentManagedAttribute.Fully())
                        .AddParameter(_template.GetTypeName(targetDto.InternalElement), "dto");

                    var attributeModels = GetDomainAttributeModels(element);

                    var attributeMap = attributeModels
                        .Select(a => (Domain: a, Dto: targetDto.Fields.FirstOrDefault(f => f.Mapping?.Element.Id == a.Id)))
                        .ToArray();
                    if (attributeMap.Any(x => x.Dto == null))
                    {
                        method.AddStatement("#warning Not all fields specified for ValueObject.");
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

        private static IList<AttributeModel> GetDomainAttributeModels(IElement element)
        {
            return element.ChildElements.Where(x => x.IsAttributeModel()).Select(x => x.AsAttributeModel()).ToList();
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
            if (_template.Model.Mapping?.Element?.IsClassConstructorModel() != true)
            {
                return NoMatch;
            }

            var ctorModel = _template.Model.Mapping.Element.AsClassConstructorModel();
            var foundEntity = ctorModel.ParentClass;
            if (foundEntity == null)
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

            return new StrategyData(true, foundEntity, repository, dtoToReturn, _template.GetAdditionalServicesFromParameters(ctorModel.Parameters), repositoryInterfaceModel);
        }

        private static readonly StrategyData NoMatch = new(false, null, null, null, null, null);

        internal class StrategyData
        {
            public StrategyData(bool isMatch, ClassModel foundEntity, RequiredService repository, DTOModel dtoToReturn, IReadOnlyCollection<RequiredService> additionalServices, ClassModel repositoryInterfaceModel)
            {
                IsMatch = isMatch;
                FoundEntity = foundEntity;
                Repository = repository;
                DtoToReturn = dtoToReturn;
                AdditionalServices = additionalServices ?? Array.Empty<RequiredService>();
                RepositoryInterfaceModel = repositoryInterfaceModel;
            }

            public bool IsMatch { get; }
            public ClassModel FoundEntity { get; }
            public RequiredService Repository { get; }
            public IReadOnlyCollection<RequiredService> AdditionalServices { get; }
            public DTOModel DtoToReturn { get; }
            public ClassModel RepositoryInterfaceModel { get; }
        }
    }
}