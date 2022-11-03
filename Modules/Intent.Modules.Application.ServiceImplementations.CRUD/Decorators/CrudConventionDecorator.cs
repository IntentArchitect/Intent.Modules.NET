using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Modelers.Domain.Api;
using Intent.Modelers.Services.Api;
using Intent.Modules.Application.ServiceImplementations.Conventions.CRUD.MethodImplementationStrategies;
using Intent.Modules.Application.ServiceImplementations.Templates.ServiceImplementation;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;
using OperationModel = Intent.Modelers.Services.Api.OperationModel;

[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateDecorator", Version = "1.0")]
[assembly: DefaultIntentManaged(Mode.Fully)]

namespace Intent.Modules.Application.ServiceImplementations.Conventions.CRUD.Decorators
{
    public class CrudConventionDecorator : ServiceImplementationDecorator
    {
        [IntentManaged(Mode.Fully)]
        public const string DecoratorId = "Intent.Application.ServiceImplementations.Conventions.CRUD.CrudConventionDecorator";

        [IntentManaged(Mode.Fully)]
        private readonly ServiceImplementationTemplate _template;
        [IntentManaged(Mode.Fully)]
        private readonly IApplication _application;
        private readonly List<IImplementationStrategy> _strategies;
        private readonly ClassModel _targetEntity;

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public CrudConventionDecorator(ServiceImplementationTemplate template, IApplication application)
        {
            _template = template;
            _application = application;
            _strategies = new List<IImplementationStrategy>
            {
                new GetAllImplementationStrategy(this),
                new GetByIdImplementationStrategy(this),
                new CreateImplementationStrategy(this),
                new UpdateImplementationStrategy(this),
                new DeleteImplementationStrategy(this),
                new GetAllPaginationImplementationStrategy(this)
            };
            _targetEntity = GetDomainForService(_template.Model);
        }

        public ServiceImplementationTemplate Template => _template;

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
            var entities = _application.MetadataManager.Domain(_application).GetClassModels();
            return entities.SingleOrDefault(e => e.Name.Equals(serviceIdentifier, StringComparison.InvariantCultureIgnoreCase) ||
                                                 e.Name.Pluralize().Equals(serviceIdentifier, StringComparison.InvariantCultureIgnoreCase));
        }

        public DTOModel FindDTOModel(string elementId)
        {
            return _application.MetadataManager.Services(_application).GetDTOModels().First(p => p.Id == elementId);
        }
    }
}
