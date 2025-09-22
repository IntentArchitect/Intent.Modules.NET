using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace AdvancedMapping.Repositories.Mapperly.Tests.Domain.Entities
{
    public class Customer
    {
        public Customer()
        {
            Name = null!;
            Surname = null!;
        }

        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Surname { get; set; }

        public DateTime DateOfBirth { get; set; }

        public virtual ICollection<Address> Addresses { get; set; } = [];
    }
}