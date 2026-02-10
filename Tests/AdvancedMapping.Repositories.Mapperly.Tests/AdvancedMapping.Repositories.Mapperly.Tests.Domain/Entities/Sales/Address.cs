using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace AdvancedMapping.Repositories.Mapperly.Tests.Domain.Entities.Sales
{
    public class Address
    {
        public Address()
        {
            Line1 = null!;
            City = null!;
            PostCode = null!;
        }

        public Guid Id { get; set; }

        public Guid CustomerId { get; set; }

        public string Line1 { get; set; }

        public string? Line2 { get; set; }

        public string City { get; set; }

        public string PostCode { get; set; }
    }
}