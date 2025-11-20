using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.Contracts.IntegrationEventMessage", Version = "1.0")]

namespace CompositePublishTest.Eventing.Messages
{
    public record ClientCreatedEvent
    {
        public ClientCreatedEvent()
        {
            Name = null!;
            Location = null!;
            Description = null!;
        }

        public string Name { get; init; }
        public string Location { get; init; }
        public string Description { get; init; }
    }
}