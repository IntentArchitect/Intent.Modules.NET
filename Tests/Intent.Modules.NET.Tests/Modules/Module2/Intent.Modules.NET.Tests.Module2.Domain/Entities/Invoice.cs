using Intent.Modules.NET.Tests.Module2.Domain.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace Intent.Modules.NET.Tests.Module2.Domain.Entities
{
    public class Invoice : IHasDomainEvent
    {
        public Invoice()
        {
            MyCustomer = null!;
        }

        public Guid Id { get; set; }

        public Guid MyCustomerId { get; set; }

        public Guid OrderId { get; set; }

        public virtual MyCustomer MyCustomer { get; set; }

        public List<DomainEvent> DomainEvents { get; set; } = [];
    }
}