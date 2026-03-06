using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace AdvancedMapping.Repositories.Mapperly.Tests.Domain.Entities.Sales
{
    public class Customer
    {
        public Customer()
        {
            Name = null!;
            Email = null!;
        }

        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Email { get; set; }

        public bool IsVip { get; set; }

        public DateTime? BirthDate { get; set; }

        public string? MetadataJson { get; set; }

        public virtual ICollection<Address> Addresses { get; set; } = [];

        public virtual Preferences? Preferences { get; set; }
    }
}