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
        private ClassModel _foundEntity;
        private RequiredService _repository;
        private DTOModel _dtoToReturn;
        private DTOFieldModel _idProperty;

        public GetByIdImplementationStrategy(QueryHandlerTemplate template, IApplication application, IMetadataManager metadataManager)
        {
            _template = template;
            _application = application;
            _metadataManager = metadataManager;
        }

        public bool IsMatch()
        {
            if (_template.Model.TypeReference.Element == null)
            {
                return false;
            }

            if (_template.Model.Properties.Count() != 1)
            {
                return false;
            }

            var matchingEntities = _metadataManager.Domain(_application).GetClassModels().Where(x => new[]
            {
                $"get{x.Name.ToLower()}",
                $"get{x.Name.ToLower()}byid",
                $"find{x.Name.ToLower()}",
                $"find{x.Name.ToLower()}byid",
                $"lookup{x.Name.ToLower()}",
                $"lookup{x.Name.ToLower()}byid",
            }.Contains(_template.Model.Name.ToLower().RemoveSuffix("query"))).ToList();

            if (matchingEntities.Count() != 1)
            {
                return false;
            }
            _foundEntity = matchingEntities.Single();

            _dtoToReturn = _metadataManager.Services(_application).GetDTOModels().SingleOrDefault(x => x.Id == _template.Model.TypeReference.Element.Id && x.IsMapped && x.Mapping.ElementId == _foundEntity.Id);
            if (_dtoToReturn == null)
            {
                return false;
            }

            _idProperty = _template.Model.Properties.FirstOrDefault(p =>
                string.Equals(p.Name, "id", StringComparison.InvariantCultureIgnoreCase) ||
                string.Equals(p.Name, $"{_foundEntity.Name}Id", StringComparison.InvariantCultureIgnoreCase));
            if (_idProperty == null)
            {
                return false;
            }

            var repositoryInterface = _template.GetTypeName(EntityRepositoryInterfaceTemplate.TemplateId, _foundEntity, new TemplateDiscoveryOptions() { ThrowIfNotFound = false });
            if (repositoryInterface == null)
            {
                return false;
            }
            _repository = new RequiredService(type: repositoryInterface, name: repositoryInterface.Substring(1).ToCamelCase());
            return true;
        }

        public IEnumerable<RequiredService> GetRequiredServices()
        {
            return new[]
            {
                _repository,
                new RequiredService(_template.UseType("AutoMapper.IMapper"), "mapper"), 
            };
        }

        public string GetImplementation()
        {
            return $@"var {_foundEntity.Name.ToCamelCase()} = await {_repository.FieldName}.FindByIdAsync(request.{_idProperty.Name.ToPascalCase()}, cancellationToken);
            return {_foundEntity.Name.ToCamelCase()}.MapTo{_template.GetTypeName("Application.Contract.Dto", _dtoToReturn)}(_mapper);";
        }
    }
}