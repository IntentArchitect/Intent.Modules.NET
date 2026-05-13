using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace EventingSubscribers.Domain.Entities
{
    /// <summary>
    /// Update Entity with Enum priority
    /// </summary>
    public class ShipTask
    {
        public Guid Id { get; set; }

        public TaskPriority Priority { get; set; }
    }
}