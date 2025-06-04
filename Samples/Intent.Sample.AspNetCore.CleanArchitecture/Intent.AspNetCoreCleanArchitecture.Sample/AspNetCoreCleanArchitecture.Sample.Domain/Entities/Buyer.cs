using AspNetCoreCleanArchitecture.Sample.Domain.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace AspNetCoreCleanArchitecture.Sample.Domain.Entities
{
    public class Buyer : IHasDomainEvent
    {
        public Buyer()
        {
            Name = null!;
            Surname = null!;
            Email = null!;
            Address = null!;
        }

        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Surname { get; set; }

        public string Email { get; set; }

        public bool IsActive { get; set; }

        public Address Address { get; set; }

        public List<DomainEvent> DomainEvents { get; set; } = [];
    }
}