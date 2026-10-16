using Intent.RoslynWeaver.Attributes;
using TestMediator.Publisher.Domain.Common;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace TestMediator.Publisher.Domain
{
    public class AuditEntry : IHasDomainEvent
    {
        public AuditEntry()
        {
            Message = null!;
        }

        public Guid Id { get; set; }

        public string Message { get; set; }

        public Guid OrderId { get; set; }

        public List<DomainEvent> DomainEvents { get; set; } = [];
    }
}