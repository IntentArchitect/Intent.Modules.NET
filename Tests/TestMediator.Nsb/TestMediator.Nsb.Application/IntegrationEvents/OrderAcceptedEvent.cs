using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.Contracts.IntegrationEventMessage", Version = "1.0")]

namespace TestMediator.Nsb.Eventing.Messages
{
    public record OrderAcceptedEvent
    {
        public Guid OrderId { get; init; }
    }
}