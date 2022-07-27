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
    class GetByIdImplementationStrategy : ICrudImplementationStrategy
    {
        private readonly QueryHandlerTemplate _template;
        private readonly IApplication _application;
        private readonly IMetadataManager _metadataManager;

        private readonly Lazy<StrategyData> _matchingElementDetails;

        public GetByIdImplementationStrategy(QueryHandlerTemplate template, IApplication application,
            IMetadataManager metadataManager)
        {
            _template = template;
            _application = application;
            _metadataManager = metadataManager;
            _matchingElementDetails = new Lazy<StrategyData>(GetMatchingElementDetails);
        }

        public bool IsMatch()
        {
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
            var foundEntity = _matchingElementDetails.Value.FoundEntity;
            var repository = _matchingElementDetails.Value.Repository;
            var idField = _matchingElementDetails.Value.IdField;
            var dtoToReturn = _matchingElementDetails.Value.DtoToReturn;

            return
                $@"var {foundEntity.Name.ToCamelCase()} = await {repository.FieldName}.FindByIdAsync(request.{idField.Name.ToPascalCase()}, cancellationToken);
            return {foundEntity.Name.ToCamelCase()}.MapTo{_template.GetDtoName(dtoToReturn)}(_mapper);";
        }

        private StrategyData GetMatchingElementDetails()
        {
            if (_template.Model.TypeReference.Element == null)
            {
                return NoMatch;
            }

            if (_template.Model.Properties.Count() != 1)
            {
                return NoMatch;
            }

            var matchingEntities = _metadataManager.Domain(_application)
                .GetClassModels().Where(x => new[]
                {
                    $"get{x.Name.ToLower()}",
                    $"get{x.Name.ToLower()}byid",
                    $"find{x.Name.ToLower()}",
                    $"find{x.Name.ToLower()}byid",
                    $"lookup{x.Name.ToLower()}",
                    $"lookup{x.Name.ToLower()}byid",
                }.Contains(_template.Model.Name.ToLower().RemoveSuffix("query")))
                .ToList();

            if (matchingEntities.Count() != 1)
            {
                return NoMatch;
            }

            var foundEntity = matchingEntities.Single();

            var dtoToReturn = _metadataManager.Services(_application).GetDTOModels().SingleOrDefault(x =>
                x.Id == _template.Model.TypeReference.Element.Id && x.IsMapped &&
                x.Mapping.ElementId == foundEntity.Id);
            if (dtoToReturn == null)
            {
                return NoMatch;
            }

            var idField = _template.Model.Properties.FirstOrDefault(p =>
                string.Equals(p.Name, "id", StringComparison.InvariantCultureIgnoreCase) ||
                string.Equals(p.Name, $"{foundEntity.Name}Id", StringComparison.InvariantCultureIgnoreCase));
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

            return new StrategyData(true, foundEntity, dtoToReturn, idField, repository);
        }

        private static readonly StrategyData NoMatch = new StrategyData(false, null, null, null, null);

        private class StrategyData
        {
            public StrategyData(bool isMatch, ClassModel foundEntity, DTOModel dtoToReturn, DTOFieldModel idField,
                RequiredService repository)
            {
                IsMatch = isMatch;
                FoundEntity = foundEntity;
                DtoToReturn = dtoToReturn;
                IdField = idField;
                Repository = repository;
            }

            public bool IsMatch { get; }
            public ClassModel FoundEntity { get; }
            public DTOModel DtoToReturn { get; }
            public DTOFieldModel IdField { get; }
            public RequiredService Repository { get; }
        }
    }
}