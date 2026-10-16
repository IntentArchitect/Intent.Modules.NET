using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.Contracts.IntegrationEventMessage", Version = "1.0")]

namespace TestMediator.Subscriber.Eventing.Messages
{
    public record ShipmentCreatedEvent
    {
        public Guid ShipmentId { get; init; }
        public Guid OrderId { get; init; }
    }
}