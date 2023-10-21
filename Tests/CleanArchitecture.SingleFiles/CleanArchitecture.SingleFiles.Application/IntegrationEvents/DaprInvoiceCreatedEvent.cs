using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.Contracts.IntegrationEventMessage", Version = "1.0")]

namespace CleanArchitecture.SingleFiles.Messages
{
    public record DaprInvoiceCreatedEvent
    {
        public DaprInvoiceCreatedEvent()
        {
            Description = null!;
        }

        public string Description { get; init; }
    }
}