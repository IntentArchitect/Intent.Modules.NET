using EventingSubscribers.Domain;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.Contracts.IntegrationEventMessage", Version = "1.0")]

namespace EventingSubscribers.Eventing.Messages
{
    public record TaskAssignedEvent
    {
        public Guid Id { get; init; }
        public TaskPriority Priority { get; init; }
    }
}