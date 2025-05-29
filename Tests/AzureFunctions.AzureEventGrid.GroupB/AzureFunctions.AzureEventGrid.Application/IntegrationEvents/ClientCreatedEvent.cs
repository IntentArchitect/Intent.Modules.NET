using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.Contracts.IntegrationEventMessage", Version = "1.0")]

namespace AzureFunctions.AzureEventGrid.GroupB.Eventing.Messages
{
    public record ClientCreatedEvent
    {
        public ClientCreatedEvent()
        {
            Name = null!;
        }

        public string Name { get; init; }
    }
}