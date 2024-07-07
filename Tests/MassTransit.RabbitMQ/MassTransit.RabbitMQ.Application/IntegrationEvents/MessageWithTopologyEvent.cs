using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.Contracts.IntegrationEventMessage", Version = "1.0")]

namespace MassTransit.RabbitMQ.Services.External
{
    public record MessageWithTopologyEvent
    {
    }
}