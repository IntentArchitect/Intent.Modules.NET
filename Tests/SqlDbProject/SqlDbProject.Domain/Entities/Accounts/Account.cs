using Intent.RoslynWeaver.Attributes;
using SqlDbProject.Domain.Common;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace SqlDbProject.Domain.Entities.Accounts
{
    public class Account : IHasDomainEvent
    {
        public Account()
        {
            Description = null!;
            AccountNumber = null!;
            ExternalReference = null!;
            AccountType = null!;
            Stakeholder = null!;
        }

        public long AccountId { get; set; }

        public string Description { get; set; }

        public string AccountNumber { get; set; }

        public string ExternalReference { get; set; }

        public int AccountTypeId { get; set; }

        public long AccountHolderId { get; set; }

        public virtual AccountType AccountType { get; set; }

        public virtual AccountHolder Stakeholder { get; set; }

        public List<DomainEvent> DomainEvents { get; set; } = [];
    }
}