using CqrsSpike.Publisher.Domain.Events;

namespace CqrsSpike.Publisher.Application.Common.Models
{
    public sealed class OrderPlacedDomainEventNotification : DomainEventNotification<OrderPlacedDomainEvent>
    {
        public OrderPlacedDomainEventNotification(OrderPlacedDomainEvent domainEvent) : base(domainEvent)
        {
        }
    }
}
