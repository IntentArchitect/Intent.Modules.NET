using CqrsSpike.Publisher.Domain.Common;
using CqrsSpike.Publisher.Domain.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.DomainEvents.DomainEvent", Version = "1.0")]

namespace CqrsSpike.Publisher.Domain.Events
{
    public class OrderPlacedDomainEvent : DomainEvent
    {
        public OrderPlacedDomainEvent(Order order)
        {
            Order = order;
        }

        public Order Order { get; }
    }
}