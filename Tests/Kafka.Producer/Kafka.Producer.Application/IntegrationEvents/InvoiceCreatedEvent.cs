using System;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.Contracts.IntegrationEventMessage", Version = "1.0")]

namespace Kafka.Producer.Eventing.Messages
{
    public record InvoiceCreatedEvent
    {
        public InvoiceCreatedEvent()
        {
            Note = null!;
        }

        public Guid Id { get; init; }
        public string Note { get; init; }
    }
}