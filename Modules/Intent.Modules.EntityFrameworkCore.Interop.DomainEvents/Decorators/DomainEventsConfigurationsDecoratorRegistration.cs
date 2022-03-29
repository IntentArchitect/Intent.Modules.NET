using System.ComponentModel;
using Intent.Engine;
using Intent.Modules.Common.Registrations;
using Intent.Modules.EntityFrameworkCore.Templates.EntityTypeConfiguration;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateDecoratorRegistration", Version = "1.0")]

namespace Intent.Modules.EntityFrameworkCore.Interop.DomainEvents.Decorators
{
    [Description(DomainEventsConfigurationsDecorator.DecoratorId)]
    public class DomainEventsConfigurationsDecoratorRegistration : DecoratorRegistration<EntityTypeConfigurationTemplate, EntityTypeConfigurationDecorator>
    {
        public override EntityTypeConfigurationDecorator CreateDecoratorInstance(EntityTypeConfigurationTemplate template, IApplication application)
        {
            return new DomainEventsConfigurationsDecorator(template, application);
        }

        public override string DecoratorId => DomainEventsConfigurationsDecorator.DecoratorId;
    }
}