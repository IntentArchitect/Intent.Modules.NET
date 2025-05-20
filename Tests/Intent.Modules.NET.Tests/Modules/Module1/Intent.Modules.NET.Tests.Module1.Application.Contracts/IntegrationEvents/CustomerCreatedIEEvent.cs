using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.Contracts.IntegrationEventMessage", Version = "1.0")]

namespace Module1.Eventing.Messages
{
    public record CustomerCreatedIEEvent
    {
        public CustomerCreatedIEEvent()
        {
            Customer = null!;
        }

        public CustomerCreatedIECustomerDto Customer { get; init; }
    }
}