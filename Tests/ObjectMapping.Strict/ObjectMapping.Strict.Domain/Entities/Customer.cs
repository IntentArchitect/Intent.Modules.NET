using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace ObjectMapping.Strict.Domain.Entities
{
    public class Customer
    {
        public Customer()
        {
            Name = null!;
        }

        public Guid Id { get; set; }

        public string Name { get; set; }

        public CustomerTier Tier { get; set; }

        public virtual Address? Address { get; set; }
    }
}