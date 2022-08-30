using System.ComponentModel;
using Intent.Engine;
using Intent.Modules.Common.Registrations;
using Intent.Modules.EntityFrameworkCore.Templates.DbContext;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateDecoratorRegistration", Version = "1.0")]

namespace Intent.Modules.Eventing.MassTransit.OutboxPattern.Decorators
{
    [Description(OutboxTableSpecification.DecoratorId)]
    public class OutboxTableSpecificationRegistration : DecoratorRegistration<DbContextTemplate, DbContextDecoratorBase>
    {
        public override DbContextDecoratorBase CreateDecoratorInstance(DbContextTemplate template, IApplication application)
        {
            return new OutboxTableSpecification(template, application);
        }

        public override string DecoratorId => OutboxTableSpecification.DecoratorId;
    }
}