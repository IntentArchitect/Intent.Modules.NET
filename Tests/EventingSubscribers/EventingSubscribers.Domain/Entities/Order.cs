using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace EventingSubscribers.Domain.Entities
{
    /// <summary>
    /// Create Entity with Enum status
    /// </summary>
    public class Order
    {
        public Guid Id { get; set; }

        public decimal Amount { get; set; }

        public OrderStatus Status { get; set; }
    }
}