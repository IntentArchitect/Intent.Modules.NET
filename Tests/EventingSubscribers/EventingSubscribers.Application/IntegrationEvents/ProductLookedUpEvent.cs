using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.Contracts.IntegrationEventMessage", Version = "1.0")]

namespace EventingSubscribers.Eventing.Messages
{
    public record ProductLookedUpEvent
    {
        public ProductLookedUpEvent()
        {
            Name = null!;
        }

        public string Name { get; init; }
    }
}