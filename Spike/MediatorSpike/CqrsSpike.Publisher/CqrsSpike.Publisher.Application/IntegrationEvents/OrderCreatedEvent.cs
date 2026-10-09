using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.Contracts.IntegrationEventMessage", Version = "1.0")]

namespace CqrsSpike.Publisher.Eventing.Messages
{
    public record OrderCreatedEvent
    {
        public OrderCreatedEvent()
        {
            CustomerName = null!;
        }

        public Guid OrderId { get; init; }
        public string CustomerName { get; init; }
        public decimal Total { get; init; }
    }
}