using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Modelers.Domain.Api;
using Intent.Modelers.Services.Api;
using Intent.Modules.Application.MediatR.CRUD.Decorators;
using Intent.Modules.Application.MediatR.Templates;
using Intent.Modules.Application.MediatR.Templates.CommandHandler;
using Intent.Modules.Application.MediatR.Templates.QueryHandler;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;
using Intent.Modules.Entities.Repositories.Api.Templates.EntityRepositoryInterface;

namespace Intent.Modules.Application.MediatR.CRUD.CrudStrategies
{
    class GetAllImplementationStrategy : ICrudImplementationStrategy
    {
        private readonly QueryHandlerTemplate _template;
        private readonly IApplication _application;
        private readonly IMetadataManager _metadataManager;
        private readonly Lazy<StrategyData> _matchingElementDetails;

        public GetAllImplementationStrategy(QueryHandlerTemplate template, IApplication application,
            IMetadataManager metadataManager)
        {
            _template = template;
            _application = application;
            _metadataManager = metadataManager;
            _matchingElementDetails = new Lazy<StrategyData>(GetMatchingElementDetails);
        }

        public bool IsMatch()
        {
            if (_template.Model.TypeReference.Element == null || !_template.Model.TypeReference.IsCollection)
            {
                return false;
            }

            return _matchingElementDetails.Value.IsMatch;
        }

        public void ApplyStrategy()
        {
            var @class = _template.CSharpFile.Classes.First();
            var ctor = @class.Constructors.First();
            var repository = _matchingElementDetails.Value.Repository;
            ctor.AddParameter(repository.Type, repository.Name.ToParameterName(), param => param.IntroduceReadonlyField())
                .AddParameter(_template.UseType("AutoMapper.IMapper"), "mapper", param => param.IntroduceReadonlyField());

            var handleMethod = @class.FindMethod("Handle");
            handleMethod.Statements.Clear();
            handleMethod.Attributes.OfType<CSharpIntentManagedAttribute>().SingleOrDefault()?.WithBodyFully();
            handleMethod.AddStatements(GetImplementation());
        }

        public IEnumerable<CSharpStatement> GetImplementation()
        {
            var foundEntity = _matchingElementDetails.Value.FoundEntity;
            var result = _matchingElementDetails.Value;
            var repository = _matchingElementDetails.Value.Repository;
            var codeLines = new CSharpStatementAggregator();
            var aggrRootOwner = foundEntity.GetAggregateRootOwner();

            if (aggrRootOwner != null)
            {
                var aggregateRootField = _template.Model.Properties.GetAggregateRootIdField(aggrRootOwner);
                if (aggregateRootField == null)
                {
                    throw new Exception($"Nested Compositional Entity {foundEntity.Name} doesn't have an Id that refers to its owning Entity {aggrRootOwner.Name}.");
                }

                codeLines.Add($"var aggregateRoot = await {repository.FieldName}.FindByIdAsync(request.{aggregateRootField.Name.ToCSharpIdentifier(CapitalizationBehaviour.AsIs)}, cancellationToken);");
                codeLines.Add($"if (aggregateRoot == null)");
                codeLines.Add(new CSharpStatementBlock()
                    .AddStatement($@"throw new InvalidOperationException($""{{nameof({_template.GetTypeName(TemplateFulfillingRoles.Domain.Entity.Primary, aggrRootOwner)})}} of Id '{{request.{aggregateRootField.Name.ToCSharpIdentifier(CapitalizationBehaviour.AsIs)}}}' could not be found"");"));

                var association = aggrRootOwner.GetNestedCompositeAssociation(foundEntity);
                codeLines.Add($@"return aggregateRoot.{association.Name.ToCSharpIdentifier(CapitalizationBehaviour.AsIs)}.MapTo{_template.GetDtoName(result.DtoToReturn)}List(_mapper);");                
                
                return codeLines.ToList();
            }

            codeLines.Add($@"var {result.FoundEntity.Name.ToCamelCase().Pluralize()} = await {result.Repository.FieldName}.FindAllAsync(cancellationToken);
            return {result.FoundEntity.Name.ToCamelCase().Pluralize()}.MapTo{_template.GetDtoName(result.DtoToReturn)}List(_mapper);");

            return codeLines.ToList();
        }

        private StrategyData GetMatchingElementDetails()
        {
            if (!_template.Model.TypeReference?.IsCollection == true)
            {
                return NoMatch;
            }

            var returnDto = _template.Model.TypeReference?.Element.AsDTOModel();

            if (returnDto?.Mapping == null)
            {
                return NoMatch;
            }

            var foundEntity = returnDto.Mapping.Element.AsClassModel();
            
            // var matchingEntities = _metadataManager.Domain(_application)
            //     .GetClassModels().Where(x => new[]
            //     {
            //         $"get{x.Name.Pluralize().ToLower()}",
            //         $"getall{x.Name.Pluralize().ToLower()}",
            //         $"find{x.Name.Pluralize().ToLower()}",
            //         $"findall{x.Name.Pluralize().ToLower()}",
            //         $"lookup{x.Name.Pluralize().ToLower()}",
            //         $"lookupall{x.Name.Pluralize().ToLower()}",
            //     }.Contains(_template.Model.Name.ToLower().RemoveSuffix("query")))
            //     .ToList();
            
            // if (matchingEntities.Count() != 1)
            // {
            //     return NoMatch;
            // }
            //
            // var foundEntity = matchingEntities.Single();

            var dtoToReturn = _metadataManager.Services(_application)
                .GetDTOModels().SingleOrDefault(x =>
                    x.Id == _template.Model.TypeReference.Element.Id && x.IsMapped &&
                    x.Mapping.ElementId == foundEntity.Id);
            if (dtoToReturn == null)
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
            return new StrategyData(true, foundEntity, dtoToReturn, repository);
        }

        private static readonly StrategyData NoMatch = new StrategyData(false, null, null, null);

        private class StrategyData
        {
            public StrategyData(bool isMatch, ClassModel foundEntity, DTOModel dtoToReturn, RequiredService repository)
            {
                IsMatch = isMatch;
                FoundEntity = foundEntity;
                DtoToReturn = dtoToReturn;
                Repository = repository;
            }

            public bool IsMatch { get; }
            public ClassModel FoundEntity { get; }
            public DTOModel DtoToReturn { get; }
            public RequiredService Repository { get; }
        }
    }
}