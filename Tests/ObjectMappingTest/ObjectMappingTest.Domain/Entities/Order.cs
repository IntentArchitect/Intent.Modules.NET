using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace ObjectMappingTest.Domain.Entities
{
    public class Order
    {
        public Order()
        {
            RefNo = null!;
            Customer = null!;
        }

        public Guid Id { get; set; }

        public string RefNo { get; set; }

        public Guid CustomerId { get; set; }

        public OrderStatus Status { get; set; }

        public PaymentStatus PaymentStatus { get; set; }

        public virtual Customer Customer { get; set; }

        public virtual ICollection<OrderLine>? Lines { get; set; } = [];

        public virtual ICollection<Tag>? Tags { get; set; } = [];

        public string GetDisplayName() => $"{RefNo} ({Status})";
    }
}