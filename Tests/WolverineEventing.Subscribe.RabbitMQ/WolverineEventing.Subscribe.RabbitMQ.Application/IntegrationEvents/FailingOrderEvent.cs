using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.Contracts.IntegrationEventMessage", Version = "1.0")]

namespace WolverineEventing.Publish.RabbitMQ.Eventing.Messages
{
    public record FailingOrderEvent
    {
        public Guid OrderId { get; init; }
    }
}