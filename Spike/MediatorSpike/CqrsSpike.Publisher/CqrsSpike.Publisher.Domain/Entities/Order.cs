using CqrsSpike.Publisher.Domain.Common;
using CqrsSpike.Publisher.Domain.Events;

namespace CqrsSpike.Publisher.Domain.Entities
{
    public class Order : IHasDomainEvent
    {
        public Order(string customerName, decimal total)
        {
            CustomerName = customerName;
            Total = total;
            Status = OrderStatus.Placed;
            DomainEvents.Add(new OrderPlacedDomainEvent(
                order: this));
        }

        /// <summary>
        /// Required by Entity Framework.
        /// </summary>
        protected Order()
        {
            CustomerName = null!;
        }

        public Guid Id { get; set; }

        public string CustomerName { get; set; }

        public decimal Total { get; set; }

        public OrderStatus Status { get; set; }

        public Guid? ShipmentId { get; set; }

        public List<DomainEvent> DomainEvents { get; set; } = [];

        public void Update(string customerName, decimal total)
        {
            CustomerName = customerName;
            Total = total;
        }

        public void MarkShipped(Guid shipmentId)
        {
            ShipmentId = shipmentId;
            Status = OrderStatus.Shipped;
        }
    }
}