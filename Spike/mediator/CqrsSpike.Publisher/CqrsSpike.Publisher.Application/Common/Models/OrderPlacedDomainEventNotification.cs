using CqrsSpike.Publisher.Domain.Events;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Mediator.DomainEvents.DomainEventNotificationType", Version = "1.0")]

namespace CqrsSpike.Publisher.Application.Common.Models
{
    public sealed class OrderPlacedDomainEventNotification : DomainEventNotification<OrderPlacedDomainEvent>
    {
        public OrderPlacedDomainEventNotification(OrderPlacedDomainEvent domainEvent) : base(domainEvent)
        {
        }
    }
}
