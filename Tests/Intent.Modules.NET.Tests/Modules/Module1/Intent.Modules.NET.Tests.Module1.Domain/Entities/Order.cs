using Intent.Modules.NET.Tests.Module1.Domain.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace Intent.Modules.NET.Tests.Module1.Domain.Entities
{
    public class Order : IHasDomainEvent
    {
        public Order()
        {
            RefNo = null!;
            Customer = null!;
        }

        public Guid Id { get; set; }

        public Guid CustomerId { get; set; }

        public string RefNo { get; set; }

        public virtual Customer Customer { get; set; }

        public virtual ICollection<OrderItem> OrderItems { get; set; } = [];

        public List<DomainEvent> DomainEvents { get; set; } = [];
    }
}