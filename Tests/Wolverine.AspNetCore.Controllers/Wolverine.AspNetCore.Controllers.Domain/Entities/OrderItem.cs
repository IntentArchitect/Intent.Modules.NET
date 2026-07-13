using Intent.RoslynWeaver.Attributes;
using Wolverine.AspNetCore.Controllers.Domain.Common;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace Wolverine.AspNetCore.Controllers.Domain.Entities
{
    public class OrderItem : IHasDomainEvent
    {
        public Guid Id { get; set; }

        public Guid ProductId { get; set; }

        public int Quantity { get; set; }

        public decimal UnitPrice { get; set; }

        public Guid OrderId { get; set; }

        public List<DomainEvent> DomainEvents { get; set; } = [];
    }
}