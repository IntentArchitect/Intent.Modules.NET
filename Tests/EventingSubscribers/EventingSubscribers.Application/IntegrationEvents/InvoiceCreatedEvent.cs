using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.Contracts.IntegrationEventMessage", Version = "1.0")]

namespace EventingSubscribers.Eventing.Messages
{
    public record InvoiceCreatedEvent
    {
        public InvoiceCreatedEvent()
        {
            Description = null!;
            BillingStreet = null!;
            BillingCity = null!;
            BillingStreet1 = null!;
            BillingCity1 = null!;
        }

        public string Description { get; init; }
        public string BillingStreet { get; init; }
        public string BillingCity { get; init; }
        public decimal Amount { get; init; }
        public string BillingStreet1 { get; init; }
        public string BillingCity1 { get; init; }
    }
}