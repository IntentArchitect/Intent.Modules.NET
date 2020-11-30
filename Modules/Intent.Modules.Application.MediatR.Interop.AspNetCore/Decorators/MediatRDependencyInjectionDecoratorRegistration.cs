using System.ComponentModel;
using Intent.Engine;
using Intent.Modules.Application.DependencyInjection.Templates.DependencyInjection;
using Intent.Modules.Common.Registrations;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("ModuleBuilder.Templates.TemplateDecoratorRegistration", Version = "1.0")]

namespace Intent.Modules.Application.MediatR.Interop.AspNetCore.Decorators
{
    [Description(MediatRDependencyInjectionDecorator.DecoratorId)]
    public class MediatRDependencyInjectionDecoratorRegistration : DecoratorRegistration<DependencyInjectionTemplate, DependencyInjectionDecorator>
    {
        public override DependencyInjectionDecorator CreateDecoratorInstance(DependencyInjectionTemplate template, IApplication application)
        {
            return new MediatRDependencyInjectionDecorator(template);
        }

        public override string DecoratorId => MediatRDependencyInjectionDecorator.DecoratorId;
    }
}