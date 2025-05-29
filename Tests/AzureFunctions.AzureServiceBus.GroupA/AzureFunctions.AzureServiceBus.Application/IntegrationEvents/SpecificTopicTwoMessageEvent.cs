using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.Contracts.IntegrationEventMessage", Version = "1.0")]

namespace AzureFunctions.AzureServiceBus.GroupB.Eventing.Messages
{
    public record SpecificTopicTwoMessageEvent
    {
        public SpecificTopicTwoMessageEvent()
        {
            FieldB = null!;
        }

        public string FieldB { get; init; }
    }
}