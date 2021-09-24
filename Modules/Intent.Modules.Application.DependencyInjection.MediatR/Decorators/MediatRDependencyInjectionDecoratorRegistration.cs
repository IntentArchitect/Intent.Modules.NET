using System.ComponentModel;
using System.Linq;
using Intent.Engine;
using Intent.Modules.Application.DependencyInjection.Templates.DependencyInjection;
using Intent.Modules.Common.Registrations;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateDecoratorRegistration", Version = "1.0")]

namespace Intent.Modules.Application.DependencyInjection.MediatR.Decorators
{
    [Description(MediatRDependencyInjectionDecorator.DecoratorId)]
    public class MediatRDependencyInjectionDecoratorRegistration : DecoratorRegistration<DependencyInjectionTemplate, DependencyInjectionDecorator>
    {
        public override DependencyInjectionDecorator CreateDecoratorInstance(DependencyInjectionTemplate template, IApplication application)
        {
            return new MediatRDependencyInjectionDecorator(template, application);
        }

        public override string DecoratorId => MediatRDependencyInjectionDecorator.DecoratorId;
    }
}