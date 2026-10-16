using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.Contracts.IntegrationEventMessage", Version = "1.0")]

namespace TestMediator.Publisher.Eventing.Messages
{
    public record OrderCancelledEvent
    {
        public Guid OrderId { get; init; }
    }
}