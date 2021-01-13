using System.ComponentModel;
using Intent.Engine;
using Intent.Modules.Common.Registrations;
using Intent.Modules.EntityFrameworkCore.Templates.Configurations;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateDecoratorRegistration", Version = "1.0")]

namespace Intent.Modules.EntityFrameworkCore.Interop.DomainEvents.Decorators
{
    [Description(DomainEventsConfigurationsDecorator.DecoratorId)]
    public class DomainEventsConfigurationsDecoratorRegistration : DecoratorRegistration<ConfigurationsTemplate, ConfigurationsDecorator>
    {
        public override ConfigurationsDecorator CreateDecoratorInstance(ConfigurationsTemplate template, IApplication application)
        {
            return new DomainEventsConfigurationsDecorator(template);
        }

        public override string DecoratorId => DomainEventsConfigurationsDecorator.DecoratorId;
    }
}