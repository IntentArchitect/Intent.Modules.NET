using System.ComponentModel;
using Intent.Engine;
using Intent.Modules.Common.Registrations;
using Intent.Modules.EntityFrameworkCore.Templates.DbContext;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateDecoratorRegistration", Version = "1.0")]

namespace Intent.Modules.EntityFrameworkCore.Interop.DomainEvents.Decorators
{
    [Description(DomainEventsDbContextDecorator.DecoratorId)]
    public class DomainEventsDbContextDecoratorRegistration : DecoratorRegistration<DbContextTemplate, ITemplateDecorator>
    {
        public override ITemplateDecorator CreateDecoratorInstance(DbContextTemplate template, IApplication application)
        {
            return new DomainEventsDbContextDecorator(template, application);
        }

        public override string DecoratorId => DomainEventsDbContextDecorator.DecoratorId;
    }
}