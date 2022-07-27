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
using Intent.Modules.Common.Templates;
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

        public IEnumerable<RequiredService> GetRequiredServices()
        {
            return new[]
            {
                _matchingElementDetails.Value.Repository,
                new RequiredService(_template.UseType("AutoMapper.IMapper"), "mapper"),
            };
        }

        public string GetImplementation()
        {
            var result = _matchingElementDetails.Value;
            return
                $@"var {result.FoundEntity.Name.ToCamelCase().ToPluralName()} = await {result.Repository.FieldName}.FindAllAsync(cancellationToken);
            return {result.FoundEntity.Name.ToCamelCase().ToPluralName()}.MapTo{_template.GetDtoName(result.DtoToReturn)}List(_mapper);";
        }

        private StrategyData GetMatchingElementDetails()
        {
            var matchingEntities = _metadataManager.Domain(_application)
                .GetClassModels().Where(x => new[]
                {
                    $"get{x.Name.ToPluralName().ToLower()}",
                    $"getall{x.Name.ToPluralName().ToLower()}",
                    $"find{x.Name.ToPluralName().ToLower()}",
                    $"findall{x.Name.ToPluralName().ToLower()}",
                    $"lookup{x.Name.ToPluralName().ToLower()}",
                    $"lookupall{x.Name.ToPluralName().ToLower()}",
                }.Contains(_template.Model.Name.ToLower().RemoveSuffix("query")))
                .ToList();

            if (matchingEntities.Count() != 1)
            {
                return NoMatch;
            }

            var foundEntity = matchingEntities.Single();

            var dtoToReturn = _metadataManager.Services(_application)
                .GetDTOModels().SingleOrDefault(x =>
                    x.Id == _template.Model.TypeReference.Element.Id && x.IsMapped &&
                    x.Mapping.ElementId == foundEntity.Id);
            if (dtoToReturn == null)
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