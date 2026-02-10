using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace AdvancedMapping.Repositories.Mapperly.Tests.Domain.Entities.Sales
{
    public class Discount
    {
        public Discount()
        {
            Description = null!;
        }

        public Guid Id { get; set; }

        public string Description { get; set; }

        public double? Percentage { get; set; }

        public decimal? FixedAmount { get; set; }
    }
}