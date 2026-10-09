using CqrsSpike.Publisher.Domain.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace CqrsSpike.Publisher.Domain.Entities
{
    public class AuditEntry : IHasDomainEvent
    {
        public AuditEntry()
        {
            Description = null!;
        }

        public Guid Id { get; set; }

        public string Description { get; set; }

        public DateTimeOffset CreatedAt { get; set; }

        public List<DomainEvent> DomainEvents { get; set; } = [];
    }
}