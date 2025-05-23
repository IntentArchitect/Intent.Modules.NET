using Intent.RoslynWeaver.Attributes;
using SqlDbProject.Domain.Common;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace SqlDbProject.Domain.Entities
{
    public class Policy : IHasDomainEvent
    {
        public Policy()
        {
            PolicyNumber = null!;
            PolicyStatus = null!;
            AccountHolder = null!;
            Product = null!;
        }

        public long PolicyId { get; set; }

        public Guid PolicyStatusId { get; set; }

        public long AccountHolderId { get; set; }

        public int ProductId { get; set; }

        public string PolicyNumber { get; set; }

        public DateTime OriginalInceptionDate { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime? ReviewDate { get; set; }

        public DateTime? ExpiryDate { get; set; }

        public bool IsDeleted { get; set; }

        public virtual PolicyStatus PolicyStatus { get; set; }

        public virtual AccountHolder AccountHolder { get; set; }

        public virtual Product Product { get; set; }

        public List<DomainEvent> DomainEvents { get; set; } = [];
    }
}