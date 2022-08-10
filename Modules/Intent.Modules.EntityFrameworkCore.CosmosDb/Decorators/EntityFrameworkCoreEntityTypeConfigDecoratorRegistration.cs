using System.ComponentModel;
using Intent.Engine;
using Intent.Modules.Common.Registrations;
using Intent.Modules.EntityFrameworkCore.Templates.EntityTypeConfiguration;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateDecoratorRegistration", Version = "1.0")]

namespace Intent.Modules.EntityFrameworkCore.CosmosDb.Decorators
{
    [Description(EntityFrameworkCoreEntityTypeConfigDecorator.DecoratorId)]
    public class EntityFrameworkCoreEntityTypeConfigDecoratorRegistration : DecoratorRegistration<EntityTypeConfigurationTemplate, EntityTypeConfigurationDecorator>
    {
        public override EntityTypeConfigurationDecorator CreateDecoratorInstance(EntityTypeConfigurationTemplate template, IApplication application)
        {
            return new EntityFrameworkCoreEntityTypeConfigDecorator(template, application);
        }

        public override string DecoratorId => EntityFrameworkCoreEntityTypeConfigDecorator.DecoratorId;
    }
}