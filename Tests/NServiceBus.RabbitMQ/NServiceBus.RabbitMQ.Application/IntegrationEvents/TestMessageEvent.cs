using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.Contracts.IntegrationEventMessage", Version = "1.0")]

namespace NServiceBus.RabbitMQ.Eventing.Messages
{
    public record TestMessageEvent
    {
    }
}