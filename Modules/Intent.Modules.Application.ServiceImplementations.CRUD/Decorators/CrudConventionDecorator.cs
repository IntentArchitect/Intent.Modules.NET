using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Modelers.Domain.Api;
using Intent.Modelers.Services.Api;
using Intent.Modules.Application.ServiceImplementations.Conventions.CRUD.MethodImplementationStrategies;
using Intent.Modules.Application.ServiceImplementations.Templates.ServiceImplementation;
using Intent.Modules.Common.Templates;
using Intent.Modules.Entities.Repositories.Api.Templates.EntityRepositoryInterface;
using OperationModel = Intent.Modelers.Services.Api.OperationModel;

namespace Intent.Modules.Application.ServiceImplementations.Conventions.CRUD.Decorators
{
    public class CrudConventionDecorator : ServiceImplementationDecoratorBase
    {
        public const string Identifier = "Intent.Conventions.ServiceImplementations.Decorator";

        public readonly ServiceImplementationTemplate Template;
        private readonly IMetadataManager _metadataManager;
        private readonly List<IImplementationStrategy> _strategies;
        private readonly ClassModel _targetEntity;

        public CrudConventionDecorator(ServiceImplementationTemplate template, IMetadataManager metadataManager)
        {
            Template = template;
            _metadataManager = metadataManager;
            _strategies = new List<IImplementationStrategy>
            {
                new GetAllImplementationStrategy(this),
                new GetByIdImplementationStrategy(this),
                new CreateImplementationStrategy(this),
                new UpdateImplementationStrategy(this),
                new DeleteImplementationStrategy(this)
            };
            _targetEntity = GetDomainForService(Template.Model);
        }

        public override IEnumerable<ConstructorParameter> GetConstructorDependencies()
        {
            var services = new List<ConstructorParameter>();
            if (_targetEntity == null)
            {
                return services;
            }

            foreach (var operationModel in Template.Model.Operations)
            {
                foreach (var strategy in _strategies)
                {
                    if (strategy.Match(_targetEntity, operationModel))
                    {
                        services.AddRange(strategy.GetRequiredServices(_targetEntity));
                    }
                }
            }
            return services;
        }

        public override string GetDecoratedImplementation(OperationModel operationModel)
        {
            if (_targetEntity == null)
            {
                return string.Empty;
            }

            foreach (var strategy in _strategies)
            {
                if (strategy.Match(_targetEntity, operationModel))
                {
                    return strategy.GetImplementation(_targetEntity, operationModel);
                }
            }

            return string.Empty;
        }

        private ClassModel GetDomainForService(ServiceModel service)
        {
            var serviceIdentifier = service.Name.RemoveSuffix("RestController", "Controller", "Service", "Manager").ToLower();
            var entities = _metadataManager.Domain(Template.Project.Application).GetClassModels();
            return entities.SingleOrDefault(e => e.Name.Equals(serviceIdentifier, StringComparison.InvariantCultureIgnoreCase) ||
                                                 e.Name.Pluralize().Equals(serviceIdentifier, StringComparison.InvariantCultureIgnoreCase));
        }

        public DTOModel FindDTOModel(string elementId)
        {
            return _metadataManager.Services(Template.Project.Application).GetDTOModels().First(p => p.Id == elementId);
        }
    }
}
