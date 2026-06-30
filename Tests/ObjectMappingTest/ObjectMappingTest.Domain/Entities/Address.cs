using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace ObjectMappingTest.Domain.Entities
{
    public class Address
    {
        public Address()
        {
            Street = null!;
            City = null!;
            PostalCode = null!;
        }

        public Guid Id { get; set; }

        public string Street { get; set; }

        public string City { get; set; }

        public string PostalCode { get; set; }
    }
}