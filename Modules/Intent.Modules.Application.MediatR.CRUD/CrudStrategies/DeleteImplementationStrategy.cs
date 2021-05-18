using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Modelers.Domain.Api;
using Intent.Modelers.Services.Api;
using Intent.Modules.Application.MediatR.CRUD.Decorators;
using Intent.Modules.Application.MediatR.Templates;
using Intent.Modules.Application.MediatR.Templates.CommandHandler;
using Intent.Modules.Common.Templates;
using Intent.Modules.Entities.Repositories.Api.Templates.EntityRepositoryInterface;

namespace Intent.Modules.Application.MediatR.CRUD.CrudStrategies
{
    class DeleteImplementationStrategy : ICrudImplementationStrategy
    {
        private readonly CommandHandlerTemplate _template;
        private readonly IApplication _application;
        private readonly IMetadataManager _metadataManager;
        private ClassModel _foundEntity;
        private RequiredService _repository;
        private DTOFieldModel _idProperty;

        public DeleteImplementationStrategy(CommandHandlerTemplate template, IApplication application, IMetadataManager metadataManager)
        {
            _template = template;
            _application = application;
            _metadataManager = metadataManager;
        }

        public bool IsMatch()
        {
            if (_template.Model.Properties.Count() != 1)
            {
                return false;
            }

            var matchingEntities = _metadataManager.Domain(_application).GetClassModels().Where(x => new[]
            {
                $"delete{x.Name.ToLower()}",
                $"remove{x.Name.ToLower()}",
            }.Contains(_template.Model.Name.ToLower().RemoveSuffix("command"))).ToList();

            if (matchingEntities.Count() != 1)
            {
                return false;
            }

            _foundEntity = matchingEntities.Single();

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
                _repository
            };
        }

        public string GetImplementation()
        {
            return $@"var existing{_foundEntity.Name} = await {_repository.FieldName}.FindByIdAsync(request.{_idProperty.Name.ToPascalCase()}, cancellationToken);
                {_repository.FieldName}.Remove(existing{_foundEntity.Name});
                return Unit.Value;";
        }
    }
}