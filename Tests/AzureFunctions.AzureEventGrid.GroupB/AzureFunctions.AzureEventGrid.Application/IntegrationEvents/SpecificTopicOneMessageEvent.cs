using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.Contracts.IntegrationEventMessage", Version = "1.0")]

namespace AzureFunctions.AzureEventGrid.GroupB.Eventing.Messages
{
    public record SpecificTopicOneMessageEvent
    {
        public SpecificTopicOneMessageEvent()
        {
            FieldA = null!;
        }

        public string FieldA { get; init; }
    }
}