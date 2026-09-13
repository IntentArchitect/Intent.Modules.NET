using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.Contracts.IntegrationEventMessage", Version = "1.0")]

namespace WolverineEventing.Subscribe.RabbitMQ.Eventing.Messages
{
    public record OrderShipmentRecordedEvent
    {
        public Guid OrderId { get; init; }
    }
}