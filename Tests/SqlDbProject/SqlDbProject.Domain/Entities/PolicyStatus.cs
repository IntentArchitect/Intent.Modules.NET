using Intent.RoslynWeaver.Attributes;
using SqlDbProject.Domain.Common;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace SqlDbProject.Domain.Entities
{
    public class PolicyStatus : IHasDomainEvent
    {
        public PolicyStatus()
        {
            Description = null!;
        }

        public Guid PolicyStatusId { get; set; }

        public string Description { get; set; }

        public List<DomainEvent> DomainEvents { get; set; } = [];
    }
}