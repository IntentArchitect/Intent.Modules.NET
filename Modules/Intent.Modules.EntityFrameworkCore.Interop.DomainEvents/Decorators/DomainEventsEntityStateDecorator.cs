using System.Collections.Generic;
using Intent.Modelers.Domain.Api;
using Intent.Modules.DomainEvents.Templates.DomainEvent;
using Intent.Modules.DomainEvents.Templates.DomainEventBase;
using Intent.Modules.DomainEvents.Templates.DomainEventServiceInterface;
using Intent.Modules.DomainEvents.Templates.HasDomainEventInterface;
using Intent.Modules.Entities.Templates.DomainEntityState;
using Intent.Modules.EntityFrameworkCore.Templates.DbContext;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateDecorator", Version = "1.0")]

namespace Intent.Modules.EntityFrameworkCore.Interop.DomainEvents.Decorators
{
    [IntentManaged(Mode.Merge)]
    public class DomainEventsEntityStateDecorator : DomainEntityStateDecoratorBase
    {
        [IntentManaged(Mode.Fully)]
        public const string DecoratorId = "Intent.EntityFrameworkCore.Interop.DomainEvents.DomainEventsEntityStateDecorator";
        public DomainEventsEntityStateDecorator(DomainEntityStateTemplate template) : base(template)
        {
        }

        public override IEnumerable<string> GetInterfaces(ClassModel @class)
        {
            yield return Template.GetTypeName(HasDomainEventInterfaceTemplate.TemplateId);
        }

        public override string AfterProperties(ClassModel @class)
        {
            return $@"

        public List<{Template.GetTypeName(DomainEventBaseTemplate.TemplateId)}> DomainEvents {{ get; set; }} = new List<{Template.GetTypeName(DomainEventBaseTemplate.TemplateId)}>();";
        }
    }
}