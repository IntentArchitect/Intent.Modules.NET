using CqrsSpike.Publisher.Domain.Common;
using CqrsSpike.Publisher.Domain.Entities;

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