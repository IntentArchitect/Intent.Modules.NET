using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.Contracts.IntegrationEventMessage", Version = "1.0")]

namespace CleanArchitecture.SingleFiles.Messages
{
    public record EfInvoiceCreatedEvent
    {
        public EfInvoiceCreatedEvent()
        {
            Description = null!;
        }

        public string Description { get; init; }
    }
}