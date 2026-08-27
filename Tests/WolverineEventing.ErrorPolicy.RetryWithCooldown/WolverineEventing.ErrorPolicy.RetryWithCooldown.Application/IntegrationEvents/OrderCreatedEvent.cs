using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.Contracts.IntegrationEventMessage", Version = "1.0")]

namespace WolverineEventing.ErrorPolicy.RetryWithCooldown.Eventing.Messages
{
    public record OrderCreatedEvent
    {
        public Guid OrderId { get; init; }
    }
}