using Intent.RoslynWeaver.Attributes;
using SqlDbProject.Domain.Common;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace SqlDbProject.Domain.Entities
{
    public class AccountHolder : IHasDomainEvent
    {
        public AccountHolder()
        {
            Name = null!;
        }

        public long AccountHolderId { get; set; }

        public string Name { get; set; }

        public DateTime DateCreated { get; set; }

        public List<DomainEvent> DomainEvents { get; set; } = [];
    }
}