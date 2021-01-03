using System.ComponentModel;
using Intent.Engine;
using Intent.Modules.Common.Registrations;
using Intent.Modules.EntityFrameworkCore.Templates.DbContext;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateDecoratorRegistration", Version = "1.0")]

namespace Intent.Modules.EntityFrameworkCore.Interop.DomainEvents.Decorators
{
    [Description(DomainEventsDbContextDecorator.DecoratorId)]
    public class DomainEventsDbContextDecoratorRegistration : DecoratorRegistration<DbContextTemplate, DbContextDecoratorBase>
    {
        public override DbContextDecoratorBase CreateDecoratorInstance(DbContextTemplate template, IApplication application)
        {
            return new DomainEventsDbContextDecorator(template);
        }

        public override string DecoratorId => DomainEventsDbContextDecorator.DecoratorId;
    }
}