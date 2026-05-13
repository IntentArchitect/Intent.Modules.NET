using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.Contracts.IntegrationEventMessage", Version = "1.0")]

namespace EventingSubscribers.Eventing.Messages
{
    public record ShipmentLookedUpEvent
    {
        public ShipmentLookedUpEvent()
        {
            DestStreet = null!;
            DestinationStreet = null!;
        }

        public string DestStreet { get; init; }
        public string DestinationStreet { get; init; }
        public Guid ShipmentId { get; init; }
    }
}