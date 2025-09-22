using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace AdvancedMapping.Repositories.Mapperly.Tests.Domain.Entities
{
    public class Address
    {
        public Address()
        {
            Line1 = null!;
            Line2 = null!;
            City = null!;
            Province = null!;
            PostalCode = null!;
            Country = null!;
        }

        public Guid Id { get; set; }

        public string Line1 { get; set; }

        public string Line2 { get; set; }

        public string City { get; set; }

        public string Province { get; set; }

        public string PostalCode { get; set; }

        public string Country { get; set; }

        public Guid CustomerId { get; set; }
    }
}