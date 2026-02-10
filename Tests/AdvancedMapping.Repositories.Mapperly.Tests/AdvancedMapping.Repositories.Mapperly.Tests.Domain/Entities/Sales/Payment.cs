using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace AdvancedMapping.Repositories.Mapperly.Tests.Domain.Entities.Sales
{
    public class Payment
    {
        public Payment()
        {
            Method = null!;
        }

        public Guid Id { get; set; }

        public Guid OrderId { get; set; }

        public string Method { get; set; }

        public decimal Amount { get; set; }

        public DateTime? PaidOn { get; set; }
    }
}