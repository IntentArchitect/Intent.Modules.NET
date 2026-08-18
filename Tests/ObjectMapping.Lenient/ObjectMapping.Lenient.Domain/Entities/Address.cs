using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace ObjectMapping.Lenient.Domain.Entities
{
    public class Address
    {
        public Address()
        {
            Line1 = null!;
            City = null!;
            PostalCode = null!;
        }

        public Guid Id { get; set; }

        public string Line1 { get; set; }

        public string City { get; set; }

        public string PostalCode { get; set; }
    }
}