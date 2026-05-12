using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.Contracts.IntegrationEventMessage", Version = "1.0")]

namespace EventingSubscribers.Eventing.Messages
{
    public record ItemCreatedEvent
    {
        public ItemCreatedEvent()
        {
            Category = null!;
            Name = null!;
        }

        public Guid ItemId { get; init; }
        public string Category { get; init; }
        public string Name { get; init; }
        public decimal Price { get; init; }
    }
}