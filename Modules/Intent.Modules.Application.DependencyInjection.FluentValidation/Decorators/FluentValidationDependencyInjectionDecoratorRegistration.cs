using System.ComponentModel;
using Intent.Engine;
using Intent.Modules.Application.DependencyInjection.Templates.DependencyInjection;
using Intent.Modules.Common.Registrations;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateDecoratorRegistration", Version = "1.0")]

namespace Intent.Modules.Application.DependencyInjection.FluentValidation.Decorators
{
    [Description(FluentValidationDependencyInjectionDecorator.DecoratorId)]
    public class FluentValidationDependencyInjectionDecoratorRegistration : DecoratorRegistration<DependencyInjectionTemplate, DependencyInjectionDecorator>
    {
        public override DependencyInjectionDecorator CreateDecoratorInstance(DependencyInjectionTemplate template, IApplication application)
        {
            return new FluentValidationDependencyInjectionDecorator(template);
        }

        public override string DecoratorId => FluentValidationDependencyInjectionDecorator.DecoratorId;
    }
}