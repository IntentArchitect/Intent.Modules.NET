using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.Contracts.IntegrationEventMessage", Version = "1.0")]

namespace AzureFunction.QueueStorage.Eventing.Messages
{
    public record ProductCreatedEvent
    {
        public ProductCreatedEvent()
        {
            Name = null!;
        }

        public string Name { get; init; }
        public int Qty { get; init; }
    }
}