using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;
using Kafka.Producer.Domain.Common;
using Kafka.Producer.Domain.Entities;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.DomainEvents.DomainEvent", Version = "1.0")]

namespace Kafka.Producer.Domain.Events
{
    public class InvoiceCreated : DomainEvent
    {
        public InvoiceCreated(Invoice invoice)
        {
            Invoice = invoice;
        }

        public Invoice Invoice { get; }
    }
}