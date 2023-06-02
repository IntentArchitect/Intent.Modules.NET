using System;
using System.Collections.Generic;
using CleanArchitecture.Dapr.Domain.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "1.0")]

namespace CleanArchitecture.Dapr.Domain.Entities
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    [DefaultIntentManaged(Mode.Fully, Targets = Targets.Properties)]
    [DefaultIntentManaged(Mode.Fully, Targets = Targets.Methods, Body = Mode.Ignore, AccessModifiers = AccessModifiers.Public)]
    public class Invoice : IHasDomainEvent
    {
        public string Id { get; set; }

        public int Number { get; set; }

        public string ClientId { get; set; }

        public virtual ICollection<InvoiceLine> InvoiceLines { get; set; } = new List<InvoiceLine>();

        public List<DomainEvent> DomainEvents { get; set; } = new List<DomainEvent>();
    }
}