using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.Contracts.IntegrationEventMessage", Version = "1.0")]

namespace AzureFunctions.AzureEventGrid.EventDomain
{
    public record OrderCreatedEvent
    {
        public OrderCreatedEvent()
        {
            RefNo = null!;
        }

        public string RefNo { get; init; }
    }
}