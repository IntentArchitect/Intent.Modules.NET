using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.Contracts.IntegrationEventMessage", Version = "1.0")]

namespace Wolverine.Publish.RabbitMQ.Eventing.Messages
{
    public record OrderShippedEvent
    {
        public Guid OrderId { get; init; }
        public DateTime ShippedAt { get; init; }
    }
}