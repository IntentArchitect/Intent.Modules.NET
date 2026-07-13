using Intent.RoslynWeaver.Attributes;
using Wolverine.AspNetCore.Controllers.Domain.Common;
using Wolverine.AspNetCore.Controllers.Domain.Events;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace Wolverine.AspNetCore.Controllers.Domain.Entities
{
    /// <summary>
    /// Order aggregate root. Raises OrderPlacedDomainEvent (no explicit handler) via PlaceOrder to exercise the default domain-event handler.
    /// </summary>
    public class Order : IHasDomainEvent
    {
        public Order()
        {
            OrderNumber = null!;
            CustomerName = null!;
            ShippingAddress = null!;
        }

        public Guid Id { get; set; }

        public string OrderNumber { get; set; }

        public OrderStatus Status { get; set; }

        public string CustomerName { get; set; }

        public DateTime PlacedDate { get; set; }

        public string? Notes { get; set; }

        public virtual ICollection<OrderItem> OrderItems { get; set; } = [];

        public ShippingAddress ShippingAddress { get; set; }

        public List<DomainEvent> DomainEvents { get; set; } = [];

        public void PlaceOrder()
        {
            DomainEvents.Add(new OrderPlacedDomainEvent(
                order: this));
        }
    }
}