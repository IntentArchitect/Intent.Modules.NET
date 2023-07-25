using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Intent.Metadata.Models;
using Intent.Modelers.Domain.Api;
using Intent.Modelers.Services.Api;
using Intent.Modules.Application.MediatR.CRUD.Decorators;
using Intent.Modules.Application.MediatR.Templates;
using Intent.Modules.Application.MediatR.Templates.CommandHandler;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;
using Intent.Modules.Entities.Repositories.Api.Templates;
using OperationModel = Intent.Modelers.Domain.Api.OperationModel;
using OperationModelExtensions = Intent.Modelers.Domain.Api.OperationModelExtensions;
using ParameterModel = Intent.Modelers.Domain.Api.ParameterModel;

namespace Intent.Modules.Application.MediatR.CRUD.CrudStrategies
{
    public class DomainOpImplementationStrategy : ICrudImplementationStrategy
    {
        private readonly CommandHandlerTemplate _template;

        private readonly Lazy<StrategyData> _matchingElementDetails;

        public DomainOpImplementationStrategy(CommandHandlerTemplate template)
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

            if (_matchingElementDetails.Value.DtoToReturn != null)
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

            var codeLines = new CSharpStatementAggregator();

            codeLines.Add($"var entity = await {repository.FieldName}.FindByIdAsync({idFields.GetEntityIdFromRequest()}, cancellationToken);");
            codeLines.Add(_template.CreateThrowNotFoundIfNullStatement(
                variable: "entity",
                message: $"Could not find {foundEntity.Name.ToPascalCase()} '{idFields.GetEntityIdFromRequestDescription()}'"));
            codeLines.Add(string.Empty);

            var dtoToReturn = _matchingElementDetails.Value.DtoToReturn;
            GenerateOperationInvocationCode("request", "entity", dtoToReturn != null);

            if (dtoToReturn != null)
            {
                codeLines.Add($@"return result.MapTo{_template.GetDtoName(dtoToReturn)}(_mapper);");
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

                foreach (var param in GetInvocationParameters(operation.Parameters, _template.Model.Properties, dtoVarName))
                {
                    invocationStatement.AddArgument(param);
                }

                if (IsAsync(operation))
                {
                    invocationStatement.AddArgument("cancellationToken");
                }

                codeLines.Add(invocationStatement);
            }
        }

        private IEnumerable<CSharpStatement> GetInvocationParameters(
            IList<ParameterModel> parameters,
            IList<DTOFieldModel> fields,
            string dtoVarName)
        {
            var list = new List<CSharpStatement>();
            foreach (var parameter in parameters)
            {
                if (parameter.TypeReference?.Element?.SpecializationType is "Value Object" or "Data Contract")
                {
                    var mappingMethodName = $"Create{parameter.TypeReference.Element.Name.ToPascalCase()}";

                    var mappedField = fields.First(field => field.Mapping?.Element?.Id == parameter.Id);
                    var constructMethod = $"{mappingMethodName}({dtoVarName}.{mappedField.Name.ToPascalCase()})";
                    list.Add(constructMethod);
                    _template.AddValueObjectFactoryMethod(mappingMethodName, (IElement)parameter.TypeReference.Element, mappedField);
                    continue;
                }

                var dtoFieldRef = fields.Where(field => field.Mapping?.Element?.Id == parameter.Id)
                    .Select(field => $"{dtoVarName}.{field.Name.ToPascalCase()}")
                    .FirstOrDefault();
                if (dtoFieldRef != null)
                {
                    list.Add(dtoFieldRef);
                    continue;
                }

                var service = _template.FindRequiredService(parameter.Type.Element);
                if (service != null)
                {
                    list.Add(service.FieldName);
                    continue;
                }

                list.Add("UNKNOWN");
            }
            return list;
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

            var idFields = _template.Model.Properties.GetEntityIdFields(foundEntity);
            if (!idFields.Any())
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

            return new StrategyData(true, foundEntity, idFields, repository, _template.Model.TypeReference.Element?.AsDTOModel(), _template.GetAdditionalServicesFromParameters(operationModel.Parameters));
        }
        private static bool IsAsync(OperationModel operation)
        {
            return operation.HasStereotype("Asynchronous") || operation.Name.EndsWith("Async");
        }

        private static readonly StrategyData NoMatch = new(false, null, null, null, null, null);

        internal class StrategyData
        {
            public StrategyData(bool isMatch, ClassModel foundEntity, List<DTOFieldModel> idFields, RequiredService repository, DTOModel dtoToReturn, IReadOnlyCollection<RequiredService> additionalServices)
            {
                IsMatch = isMatch;
                FoundEntity = foundEntity;
                IdFields = idFields;
                Repository = repository;
                DtoToReturn = dtoToReturn;
                AdditionalServices = additionalServices ?? Array.Empty<RequiredService>();
            }

            public bool IsMatch { get; }
            public ClassModel FoundEntity { get; }
            public List<DTOFieldModel> IdFields { get; }
            public RequiredService Repository { get; }
            public IReadOnlyCollection<RequiredService> AdditionalServices { get; }
            public DTOModel DtoToReturn { get; }

        }
    }
}