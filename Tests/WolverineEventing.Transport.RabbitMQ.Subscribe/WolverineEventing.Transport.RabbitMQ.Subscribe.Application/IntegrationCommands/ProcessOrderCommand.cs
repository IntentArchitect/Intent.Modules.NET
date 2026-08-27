using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.Contracts.IntegrationCommand", Version = "1.0")]

namespace WolverineEventing.Transport.RabbitMQ.Subscribe.Eventing.Messages
{
    public record ProcessOrderCommand
    {
        public Guid OrderId { get; init; }
    }
}