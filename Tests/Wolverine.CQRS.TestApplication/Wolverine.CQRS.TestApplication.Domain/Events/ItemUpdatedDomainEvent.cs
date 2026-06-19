using Intent.RoslynWeaver.Attributes;
using Wolverine.CQRS.TestApplication.Domain.Common;
using Wolverine.CQRS.TestApplication.Domain.Entities.Items;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.DomainEvents.DomainEvent", Version = "1.0")]

namespace Wolverine.CQRS.TestApplication.Domain.Events
{
    public class ItemUpdatedDomainEvent : DomainEvent
    {
        public ItemUpdatedDomainEvent(Item item)
        {
            Item = item;
        }

        public Item Item { get; }
    }
}