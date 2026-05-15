using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.Contracts.IntegrationEventMessage", Version = "1.0")]

namespace EventingSubscribers.Eventing.Messages
{
    public record ShipmentReceivedEvent
    {
        public ShipmentReceivedEvent()
        {
            Title = null!;
            DestinationStreet = null!;
            DestinationCity = null!;
        }

        public string Title { get; init; }
        public string DestinationStreet { get; init; }
        public string DestinationCity { get; init; }
        public Guid ShipmentId { get; init; }
    }
}