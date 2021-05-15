using System.ComponentModel;
using Intent.Engine;
using Intent.Modules.Application.ServiceImplementations.Templates.ServiceImplementation;
using Intent.Modules.Common.Registrations;

namespace Intent.Modules.Application.ServiceImplementations.Conventions.CRUD.Decorators
{
    [Description(CrudConventionDecorator.Identifier)]
    public class CrudConventionDecoratorRegistration : DecoratorRegistration<ServiceImplementationTemplate, ServiceImplementationDecoratorBase>
    {
        public override string DecoratorId => CrudConventionDecorator.Identifier;

        private readonly IMetadataManager _metadataManager;

        public CrudConventionDecoratorRegistration(IMetadataManager metadataManager)
        {
            _metadataManager = metadataManager;
        }

        public override ServiceImplementationDecoratorBase CreateDecoratorInstance(ServiceImplementationTemplate template, IApplication application)
        {
            return new CrudConventionDecorator(template, _metadataManager);
        }
    }
}
