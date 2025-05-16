using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace Intent.Modules.NET.Tests.Module1.Domain.Entities
{
    public class OrderItem
    {
        public Guid Id { get; set; }

        public Guid OrderId { get; set; }

        public int Quantiity { get; set; }

        public decimal Amount { get; set; }
    }
}