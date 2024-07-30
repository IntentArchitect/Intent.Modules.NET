using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;
using OperationModel = Intent.Modelers.Domain.Api.OperationModel;
using OperationModelExtensions = Intent.Modelers.Domain.Api.OperationModelExtensions;
using ParameterModel = Intent.Modelers.Domain.Api.ParameterModel;

namespace Intent.Modules.Application.MediatR.CRUD.CrudStrategies
{
    public class DomainOpImplementationStrategy : ICrudImplementationStrategy
    {
        private readonly CSharpTemplateBase<CommandModel> _template;

        private readonly Lazy<StrategyData> _matchingElementDetails;

        public DomainOpImplementationStrategy(CSharpTemplateBase<CommandModel> template)
        {
            _template = template;
            _matchingElementDetails = new Lazy<StrategyData>(GetMatchingElementDetails);
        }
        public void BindToTemplate(ICSharpFileBuilderTemplate template)
        {
            template.CSharpFile.AfterBuild(_ => ApplyStrategy());
        }

        public bool IsMatch()
        {
            return _matchingElementDetails.Value.IsMatch;
        }

        internal StrategyData GetStrategyData() => _matchingElementDetails.Value;

        public void ApplyStrategy()
        {
            _template.AddTypeSource(TemplateRoles.Domain.Entity.Primary);
            _template.AddTypeSource(TemplateRoles.Domain.ValueObject);
            _template.AddTypeSource(TemplateRoles.Domain.DataContract);

            _template.AddUsing("System.Linq");

            var @class = ((ICSharpFileBuilderTemplate)_template).CSharpFile.Classes.First(x => x.HasMetadata("handler"));
            var ctor = @class.Constructors.First();
            var repository = _matchingElementDetails.Value.Repository;
            ctor.AddParameter(repository.Type, repository.Name.ToParameterName(),
                param => param.IntroduceReadonlyField());

            if (_matchingElementDetails.Value.CommandReturnType.Element?.IsDTOModel() == true)
            {
                ctor.AddParameter(_template.UseType("AutoMapper.IMapper"), "mapper", param => param.IntroduceReadonlyField());
            }

            foreach (var requiredService in _matchingElementDetails.Value.AdditionalServices)
            {
                ctor.AddParameter(requiredService.Type, requiredService.Name, c => c.IntroduceReadonlyField());
            }

            var handleMethod = @class.FindMethod("Handle");
            handleMethod.Statements.Clear();
            handleMethod.Attributes.OfType<CSharpIntentManagedAttribute>().SingleOrDefault()?.WithBodyFully();
            handleMethod.AddStatements(GetImplementation());
        }

        public IEnumerable<CSharpStatement> GetImplementation()
        {
            var repository = _matchingElementDetails.Value.Repository;
            var idFields = _matchingElementDetails.Value.IdFields;
            var foundEntity = _matchingElementDetails.Value.FoundEntity;
            var entityVariableName = foundEntity.GetExistingVariableName();
            var commandReturnType = _matchingElementDetails.Value.CommandReturnType;
            var operationReturnType = _matchingElementDetails.Value.OperationReturnType;

            var codeLines = new CSharpStatementAggregator();

            var nestedCompOwner = _matchingElementDetails.Value.FoundEntity.GetNestedCompositionalOwner();
            if (nestedCompOwner != null)
            {
                var aggregateRootIdFields = _template.Model.Properties.GetCompositesOwnerIdFieldsForOperations(nestedCompOwner, foundEntity, _template.ExecutionContext);
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

                codeLines.Add($"var {entityVariableName} = aggregateRoot.{association.Name.ToCSharpIdentifier(CapitalizationBehaviour.AsIs)}.FirstOrDefault({idFields.GetPropertyToRequestMatchClause()});");
                codeLines.Add(_template.CreateThrowNotFoundIfNullStatement(
                    variable: entityVariableName,
                    message: $"{{nameof({_template.GetTypeName(TemplateRoles.Domain.Entity.Primary, foundEntity)})}} of Id '{idFields.GetEntityIdFromRequestDescription()}' could not be found associated with {{nameof({_template.GetTypeName(TemplateRoles.Domain.Entity.Primary, nestedCompOwner)})}} of Id '{aggregateRootIdFields.GetEntityIdFromRequestDescription()}'"));
                codeLines.Add(string.Empty);
            }
            else
            {
                codeLines.Add($"var {entityVariableName} = await {repository.FieldName}.FindByIdAsync({idFields.GetEntityIdFromRequest(_template.Model.InternalElement)}, cancellationToken);");
                codeLines.Add(_template.CreateThrowNotFoundIfNullStatement(
                    variable: entityVariableName,
                    message: $"Could not find {foundEntity.Name.ToPascalCase()} '{idFields.GetEntityIdFromRequestDescription()}'"));
                codeLines.Add(string.Empty);
            }

            GenerateOperationInvocationCode("request", entityVariableName, operationReturnType?.Element != null);

            if (commandReturnType?.Element.IsDTOModel() == true)
            {
                codeLines.Add($"return result.MapTo{_template.GetDtoName(commandReturnType.Element.AsDTOModel())}(_mapper);");
            }
            else if (commandReturnType?.Element != null &&
                     commandReturnType.Element.Id == operationReturnType?.Element?.Id)
            {
                codeLines.Add("return result;");
            }
            else if (commandReturnType?.Element != null)
            {
                codeLines.Add("#warning Return type does not match any known convention");
                codeLines.Add("throw new NotImplementedException();");
            }

            if (RepositoryRequiresExplicitUpdate())
            {
                codeLines.Add(new CSharpStatement($"{repository.FieldName}.Update({entityVariableName});").SeparatedFromPrevious());
            }

            return codeLines.ToList();

            void GenerateOperationInvocationCode(string dtoVarName, string entityVarName, bool hasReturn)
            {
                var operation = OperationModelExtensions.AsOperationModel(_template.Model.Mapping.Element);
                if (operation == null)
                {
                    return;
                }

                var sb = new StringBuilder();
                if (hasReturn)
                {
                    sb.Append("var result = ");
                }

                if (IsAsync(operation))
                {
                    sb.Append("await ");
                }

                sb.Append(entityVarName);
                sb.Append('.');
                sb.Append(operation.Name.ToPascalCase());

                var invocationStatement = new CSharpInvocationStatement(sb.ToString());

                foreach (var param in operation.Parameters)
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

                if (IsAsync(operation))
                {
                    invocationStatement.AddArgument("cancellationToken");
                }

                codeLines.Add(invocationStatement);
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

                var mappedField = fields.First(field => field.Mapping?.Element?.Id == parameter.Id);
                var constructMethod = parameter.TypeReference.IsCollection
                    ? $"{dtoVarName}.{mappedField.Name.ToPascalCase()}.Select({mappingMethodName})"
                    : $"{mappingMethodName}({dtoVarName}.{mappedField.Name.ToPascalCase()})";
                ((ICSharpFileBuilderTemplate)_template).AddValueObjectFactoryMethod(mappingMethodName, (IElement)parameter.TypeReference.Element, mappedField);
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
            if (_template.Model.Mapping?.Element == null || !OperationModelExtensions.IsOperationModel(_template.Model.Mapping.Element))
            {
                return NoMatch;
            }

            var operationModel = OperationModelExtensions.AsOperationModel(_template.Model.Mapping.Element);
            var foundEntity = operationModel.ParentClass;
            if (foundEntity == null)
            {
                return NoMatch;
            }

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

            return new StrategyData(
                isMatch: true,
                foundEntity: foundEntity,
                idFields: idFields,
                repository: repository,
                additionalServices: _template.GetAdditionalServicesFromParameters(operationModel.Parameters),
                commandReturnType: _template.Model.TypeReference,
                operationReturnType: operationModel.TypeReference,
                repositoryInterfaceModel: repositoryInterfaceModel);
        }
        private static bool IsAsync(OperationModel operation)
        {
            return operation.HasStereotype("Asynchronous") || operation.Name.EndsWith("Async");
        }

        private static readonly StrategyData NoMatch = new(
            isMatch: false,
            foundEntity: null,
            idFields: null,
            repository: null,
            commandReturnType: null,
            operationReturnType: null,
            additionalServices: null,
            repositoryInterfaceModel: null);

        internal class StrategyData
        {
            public StrategyData(
                bool isMatch,
                ClassModel foundEntity,
                List<DTOFieldModel> idFields,
                RequiredService repository,
                ITypeReference commandReturnType,
                ITypeReference operationReturnType,
                IReadOnlyCollection<RequiredService> additionalServices,
                ClassModel repositoryInterfaceModel)
            {
                IsMatch = isMatch;
                FoundEntity = foundEntity;
                IdFields = idFields;
                Repository = repository;
                CommandReturnType = commandReturnType;
                OperationReturnType = operationReturnType;
                AdditionalServices = additionalServices ?? Array.Empty<RequiredService>();
                RepositoryInterfaceModel = repositoryInterfaceModel;
            }

            public bool IsMatch { get; }
            public ClassModel FoundEntity { get; }
            public List<DTOFieldModel> IdFields { get; }
            public RequiredService Repository { get; }
            public ITypeReference CommandReturnType { get; }
            public ITypeReference OperationReturnType { get; }
            public IReadOnlyCollection<RequiredService> AdditionalServices { get; }
            public ClassModel RepositoryInterfaceModel { get; }
        }
    }
}