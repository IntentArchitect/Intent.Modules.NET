using System.ComponentModel;
using Intent.Engine;
using Intent.Modules.Common.Registrations;
using Intent.Modules.Infrastructure.DependencyInjection.Templates.DependencyInjection;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateDecoratorRegistration", Version = "1.0")]

namespace Intent.Modules.DependencyInjection.EntityFrameworkCore.Decorators
{
    [Description(EntityFrameworkCoreDependencyInjectionDecorator.DecoratorId)]
    public class EntityFrameworkCoreDependencyInjectionDecoratorRegistration : DecoratorRegistration<DependencyInjectionTemplate, DependencyInjectionDecorator>
    {
        public override DependencyInjectionDecorator CreateDecoratorInstance(DependencyInjectionTemplate template, IApplication application)
        {
            return new EntityFrameworkCoreDependencyInjectionDecorator(template);
        }

        public override string DecoratorId => EntityFrameworkCoreDependencyInjectionDecorator.DecoratorId;
    }
}