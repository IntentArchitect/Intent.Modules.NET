using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace ObjectMapping.Strict.Domain.Entities
{
    /// <summary>
    /// Aggregate root for the reference model. Carries every mapping shape the Object Mapper module must generate.
    /// </summary>
    public class Order
    {
        public Order()
        {
            OrderNumber = null!;
            Customer = null!;
        }

        public Guid Id { get; set; }

        public string OrderNumber { get; set; }

        public OrderStatus Status { get; set; }

        public string? Notes { get; set; }

        public Guid CustomerId { get; set; }

        public virtual Customer Customer { get; set; }

        public virtual Coupon? Coupon { get; set; }

        public virtual ICollection<OrderLine>? OrderLines { get; set; } = [];

        public virtual ICollection<Tag>? Tags { get; set; } = [];

        public virtual ICollection<PaymentMethod>? PaymentMethods { get; set; } = [];

        /// <summary>
        /// Returns a human-readable label for the order, combining the order number and status. Parameterless so it can appear in a mapping path.
        /// </summary>
        public string GetDisplayLabel()
        {
            return $"Order {OrderNumber} [{Status}]";
        }
    }
}
