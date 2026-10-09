using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.Contracts.IntegrationCommand", Version = "1.0")]

namespace CqrsSpike.Subscriber.Eventing.Messages
{
    public record RequestShipmentCommand
    {
        public Guid OrderId { get; init; }
    }
}