using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.Contracts.IntegrationEventMessage", Version = "1.0")]

namespace TestMediator.WolverineSubscribeOnly.Eventing.Messages
{
    public record OrderPlacedEvent
    {
        public OrderPlacedEvent()
        {
            CustomerName = null!;
        }

        public Guid OrderId { get; init; }
        public string CustomerName { get; init; }
    }
}