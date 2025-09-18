using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.Contracts.IntegrationEventMessage", Version = "1.0")]

namespace AspNetCore.AzureServiceBus.GroupB.Eventing.Messages
{
    public record PublishAndConsumeEvent
    {
        public PublishAndConsumeEvent()
        {
            Data = null!;
        }

        public string Data { get; init; }
    }
}