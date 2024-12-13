using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.Contracts.IntegrationEventMessage", Version = "1.0")]

namespace MassTransit.AzureServiceBus.Services.External
{
    public record TestMessageEvent
    {
        public TestMessageEvent()
        {
            Message = null!;
        }
        public string Message { get; init; }
    }
}