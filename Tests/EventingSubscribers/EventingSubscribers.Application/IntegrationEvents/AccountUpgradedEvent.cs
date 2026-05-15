using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.Contracts.IntegrationEventMessage", Version = "1.0")]

namespace EventingSubscribers.Eventing.Messages
{
    public record AccountUpgradedEvent
    {
        public AccountUpgradedEvent()
        {
            NewTier = null!;
        }

        public string NewTier { get; init; }
        public Guid Id { get; init; }
    }
}