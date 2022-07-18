using System.ComponentModel;
using Intent.Engine;
using Intent.Modules.Application.ServiceImplementations.Templates.ServiceImplementation;
using Intent.Modules.Common.Registrations;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateDecoratorRegistration", Version = "1.0")]

namespace Intent.Modules.Application.ServiceImplementations.Conventions.CRUD.Decorators
{
    [Description(CrudConventionDecorator.DecoratorId)]
    public class CrudConventionDecoratorRegistration : DecoratorRegistration<ServiceImplementationTemplate, ServiceImplementationDecorator>
    {
        public override ServiceImplementationDecorator CreateDecoratorInstance(ServiceImplementationTemplate template, IApplication application)
        {
            return new CrudConventionDecorator(template, application);
        }

        public override string DecoratorId => CrudConventionDecorator.DecoratorId;
    }
}