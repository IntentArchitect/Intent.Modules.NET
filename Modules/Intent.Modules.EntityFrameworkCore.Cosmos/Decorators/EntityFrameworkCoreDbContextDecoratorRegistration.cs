using System.ComponentModel;
using Intent.Engine;
using Intent.Modules.Common.Registrations;
using Intent.Modules.EntityFrameworkCore.Templates.EntityTypeConfiguration;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateDecoratorRegistration", Version = "1.0")]

namespace Intent.Modules.EntityFrameworkCore.Cosmos.Decorators
{
    [Description(EntityFrameworkCoreDbContextDecorator.DecoratorId)]
    public class EntityFrameworkCoreDbContextDecoratorRegistration : DecoratorRegistration<EntityTypeConfigurationTemplate, ITemplateDecorator>
    {
        public override ITemplateDecorator CreateDecoratorInstance(EntityTypeConfigurationTemplate template, IApplication application)
        {
            return new EntityFrameworkCoreDbContextDecorator(template, application);
        }

        public override string DecoratorId => EntityFrameworkCoreDbContextDecorator.DecoratorId;
    }
}