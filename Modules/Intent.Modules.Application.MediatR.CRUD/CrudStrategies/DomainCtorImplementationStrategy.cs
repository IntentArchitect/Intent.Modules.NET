using System;
using System.Collections.Generic;
using System.Linq;
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
using ParameterModel = Intent.Modelers.Domain.Api.ParameterModel;
using ParameterModelExtensions = Intent.Modelers.Domain.Api.ParameterModelExtensions;

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

            var entityName = _template.GetDomainEntityName(foundEntity) ?? foundEntity.Name;

            var codeLines = new CSharpStatementAggregator();

            codeLines.Add(GetConstructorStatement($"entity", entityName, "request", false));

            codeLines.Add($"{repository.FieldName}.Add(entity);", x => x.SeparatedFromPrevious());

            if (_template.Model.TypeReference.Element != null)
            {
                codeLines.Add($"await {repository.FieldName}.UnitOfWork.SaveChangesAsync(cancellationToken);");

                var dtoToReturn = _matchingElementDetails.Value.DtoToReturn;
                codeLines.Add(dtoToReturn != null
                    ? $"return entity.MapTo{_template.GetDtoName(dtoToReturn)}(_mapper);"
                    : $"return entity.{(foundEntity.Attributes).Concat(foundEntity.ParentClass?.Attributes ?? new List<AttributeModel>()).FirstOrDefault(x => x.IsPrimaryKey())?.Name.ToPascalCase() ?? "Id"};");
            }
            else
            {
                codeLines.Add($"return Unit.Value;");
            }

            return codeLines.ToList();

            CSharpStatement GetConstructorStatement(string entityVarName, string entityName, string dtoVarName, bool hasInitStatements)
            {
                var ctor = ClassConstructorModelExtensions.AsClassConstructorModel(_template.Model.Mapping.Element);
                var ctorParams = ctor?.Parameters; 

                if (ctorParams?.Any() != true)
                {
                    return $"var {entityVarName} = new {entityName}{(hasInitStatements ? "" : "();")}";
                }

                var paramList = GetInvocationParameters(ctorParams, _template.Model.Properties, dtoVarName);
                var statement = new CSharpInvocationStatement($@"var {entityVarName} = new {entityName}");
                if (hasInitStatements)
                {
                    statement.WithoutSemicolon();
                }
                foreach (var param in paramList)
                {
                    statement.AddArgument(param);
                }

                return statement;
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
                if (parameter.TypeReference?.Element?.SpecializationType == "Value Object" || parameter.TypeReference?.Element?.SpecializationType == "Data Contract")
                {
                    var constructMethod = fields.Where(field => field.Mapping?.Element?.Id == parameter.Id)
                        .Select(field => $"Create{parameter.TypeReference.Element.Name}({dtoVarName}.{field.Name.ToPascalCase()})")
                        .First();
                    list.Add(constructMethod);
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
            if (_template.Model.Mapping?.Element == null || !ClassConstructorModelExtensions.IsClassConstructorModel(_template.Model.Mapping.Element))
            {
                return NoMatch;
            }

            var ctorModel = ClassConstructorModelExtensions.AsClassConstructorModel(_template.Model.Mapping.Element);
            var foundEntity = ctorModel.ParentClass;
            if (foundEntity == null)
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

            var dtoToReturn = _template.Model.TypeReference.Element?.AsDTOModel();

            return new StrategyData(true, foundEntity, repository, dtoToReturn, _template.GetAdditionalServicesFromParameters(ctorModel.Parameters));
        }

        private static readonly StrategyData NoMatch = new StrategyData(false, null, null, null, null);

        internal class StrategyData
        {
            public StrategyData(bool isMatch, ClassModel foundEntity, RequiredService repository, DTOModel dtoToReturn, IReadOnlyCollection<RequiredService> additionalServices)
            {
                IsMatch = isMatch;
                FoundEntity = foundEntity;
                Repository = repository;
                DtoToReturn = dtoToReturn;
                AdditionalServices = additionalServices ?? Array.Empty<RequiredService>();
            }

            public bool IsMatch { get; }
            public ClassModel FoundEntity { get; }
            public RequiredService Repository { get; }
            public IReadOnlyCollection<RequiredService> AdditionalServices { get; }
            public DTOModel DtoToReturn { get; }
        }
    }
}