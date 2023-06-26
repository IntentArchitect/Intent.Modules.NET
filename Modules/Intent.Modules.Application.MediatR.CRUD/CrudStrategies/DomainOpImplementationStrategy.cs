using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modelers.Domain.Api;
using Intent.Modelers.Domain.ValueObjects.Api;
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
using ParameterModel = Intent.Modelers.Domain.Api.ParameterModel;
using ParameterModelExtensions = Intent.Modelers.Domain.Api.ParameterModelExtensions;

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
            var idField = _matchingElementDetails.Value.IdField;

            var codeLines = new CSharpStatementAggregator();

            codeLines.Add($"var entity = await {repository.FieldName}.FindByIdAsync(request.{idField.Name.ToPascalCase()}, cancellationToken);");

            var dtoToReturn = _matchingElementDetails.Value.DtoToReturn;
            GenerateOperationInvocationCode("request", $"entity", dtoToReturn != null);

            codeLines.Add(dtoToReturn != null
                ? $@"return result.MapTo{_template.GetDtoName(dtoToReturn)}(_mapper);"
                : $"return Unit.Value;");

            return codeLines.ToList();

            void GenerateOperationInvocationCode(string dtoVarName, string entityVarName, bool hasReturn)
            {
                var oper = OperationModelExtensions.AsOperationModel(_template.Model.Mapping.Element);
                if (oper == null)
                {
                    return;
                }

                var operParams = oper.Parameters;
                var paramList = GetInvocationParameters(operParams, _template.Model.Properties, dtoVarName);
                var statement = new CSharpInvocationStatement($@"{(hasReturn ? "var result = " : "")}{entityVarName}.{oper.Name.ToPascalCase()}");
                foreach (var param in paramList)
                {
                    statement.AddArgument(param);
                }
                codeLines.Add(statement);
            }
        }

        private IEnumerable<CSharpStatement> GetInvocationParameters(
            IList<ParameterModel> operParameters, 
            IList<DTOFieldModel> fields, 
            string dtoVarName)
        {
            var list = new List<CSharpStatement>();
            foreach (var parameter in operParameters)
            {
                if (parameter.TypeReference?.Element?.SpecializationType is "Value Object" or "Data Contract")
                {
                    string mappingMethodName = $"Create{parameter.TypeReference.Element.Name}";

                    var mappedField = fields.Where(field => field.Mapping?.Element?.Id == parameter.Id).First();
                    var constructMethod = $"{mappingMethodName}({dtoVarName}.{mappedField.Name.ToPascalCase()})";
                    list.Add(constructMethod);
                    AddMappingMethod(mappingMethodName, mappedField, parameter.TypeReference.Element as IElement);
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

        private void AddMappingMethod(string mappingMethodName, DTOFieldModel field, IElement element)
        {
            var @class = _template.CSharpFile.Classes.First();
            if (@class.FindMethod(mappingMethodName) == null)
            {
                var domainType = _template.GetTypeName(element);
                var targetDto = field.TypeReference.Element.AsDTOModel();
                @class.AddMethod(domainType, mappingMethodName, method =>
                {
                    method.Static()
                        .AddAttribute(CSharpIntentManagedAttribute.Fully())
                        .AddParameter(_template.GetTypeName(targetDto.InternalElement), "dto");

                    var attributeModels = GetDomainAttibuteModels(element);
                    var ctorParameters = string.Join(",", attributeModels.Select(a => $"{a.Name.ToParameterName()}: dto.{targetDto.Fields.First(f => f.Mapping?.Element.Id == a.Id).Name.ToPascalCase()}"));
                    method.AddStatement($"return new {domainType}({ctorParameters});");
                });
            }
        }

        private IList<AttributeModel> GetDomainAttibuteModels(IElement element)
        {

            if (element.IsDataContractModel())
            {
                return element.AsDataContractModel().Attributes;
            }
            if (element.IsValueObjectModel())
            {
                return element.AsValueObjectModel().Attributes;
            }
            return new List<AttributeModel>();
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
            
            var idField = _template.Model.Properties.GetEntityIdField(foundEntity);
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

            return new StrategyData(true, foundEntity, idField, repository, _template.Model.TypeReference.Element?.AsDTOModel(), _template.GetAdditionalServicesFromParameters(operationModel.Parameters));
        }

        private static readonly StrategyData NoMatch = new StrategyData(false, null, null, null, null, null);

        internal class StrategyData
        {
            public StrategyData(bool isMatch, ClassModel foundEntity, DTOFieldModel idField, RequiredService repository, DTOModel dtoToReturn, IReadOnlyCollection<RequiredService> additionalServices)
            {
                IsMatch = isMatch;
                FoundEntity = foundEntity;
                IdField = idField;
                Repository = repository;
                DtoToReturn = dtoToReturn;
                AdditionalServices = additionalServices ?? Array.Empty<RequiredService>();
            }

            public bool IsMatch { get; }
            public ClassModel FoundEntity { get; }
            public DTOFieldModel IdField { get; }
            public RequiredService Repository { get; }
            public IReadOnlyCollection<RequiredService> AdditionalServices { get; }
            public DTOModel DtoToReturn { get; }

        }
    }
}