using System.ComponentModel;
using Intent.Engine;
using Intent.Modules.Common.Registrations;
using Intent.Modules.EntityFrameworkCore.Templates.DbContext;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateDecoratorRegistration", Version = "1.0")]

namespace Intent.Modules.Eventing.MassTransit.EntityFrameworkCore.Decorators
{
    [IntentManaged(Mode.Fully, Signature = Mode.Merge)]
    [Description(OutboxTableSpecification.DecoratorId)]
    public class OutboxTableSpecificationRegistration : DecoratorRegistration<DbContextTemplate, ITemplateDecorator>
    {
        [IntentManaged(Mode.Fully, Signature = Mode.Ignore)]
        public override ITemplateDecorator CreateDecoratorInstance(DbContextTemplate template, IApplication application)
        {
            return new OutboxTableSpecification(template, application);
        }

        public override string DecoratorId => OutboxTableSpecification.DecoratorId;
    }
}