using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.Contracts.IntegrationEventMessage", Version = "1.0")]

namespace MassTransit.RabbitMQ.Services.NamingOverrides
{
    public record OverrideMessageStandardSubscribeEvent
    {
        public OverrideMessageStandardSubscribeEvent()
        {
            Message = null!;
        }

        public string Message { get; init; }
    }
}