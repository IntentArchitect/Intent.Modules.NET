using Intent.RoslynWeaver.Attributes;
using SqlDbProject.Domain.Common;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace SqlDbProject.Domain.Entities.Accounts
{
    public class AccountType : IHasDomainEvent
    {
        public AccountType()
        {
            Description = null!;
        }

        public int AccountTypeId { get; set; }

        public string Description { get; set; }

        public List<DomainEvent> DomainEvents { get; set; } = [];
    }
}