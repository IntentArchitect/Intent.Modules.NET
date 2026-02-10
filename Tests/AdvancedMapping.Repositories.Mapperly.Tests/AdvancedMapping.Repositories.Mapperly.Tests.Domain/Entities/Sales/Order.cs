using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace AdvancedMapping.Repositories.Mapperly.Tests.Domain.Entities.Sales
{
    public class Order
    {
        public Order()
        {
            Status = null!;
            Customer = null!;
        }

        public Guid Id { get; set; }

        public Guid CustomerId { get; set; }

        public DateTime OrderDate { get; set; }

        public DateTime? RequiredBy { get; set; }

        public string Status { get; set; }

        public decimal TotalAmount { get; set; }

        public virtual Customer Customer { get; set; }

        public virtual ICollection<OrderLine> Lines { get; set; } = [];

        public virtual ICollection<Discount> Discounts { get; set; } = [];

        public virtual ICollection<Payment> Payments { get; set; } = [];

        public virtual ICollection<Shipment> Shipments { get; set; } = [];

        public bool IsActive()
        {
            // TODO: Implement IsActive (Order) functionality
            throw new NotImplementedException("Replace with your implementation...");
        }
    }
}