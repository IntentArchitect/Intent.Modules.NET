using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace ObjectMappingTest.Domain.Entities
{
    public class Customer
    {
        public Customer()
        {
            Name = null!;
        }

        public Guid Id { get; set; }

        public string Name { get; set; }

        public string? Email { get; set; }

        public virtual Address? Address { get; set; }

        public virtual Address? ShippingAddress { get; set; }
    }
}