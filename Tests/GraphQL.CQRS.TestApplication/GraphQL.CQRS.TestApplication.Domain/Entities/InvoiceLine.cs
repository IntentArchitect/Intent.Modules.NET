using System;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "1.0")]

namespace GraphQL.CQRS.TestApplication.Domain.Entities
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    [DefaultIntentManaged(Mode.Fully, Targets = Targets.Properties)]
    [DefaultIntentManaged(Mode.Fully, Targets = Targets.Methods, Body = Mode.Ignore, AccessModifiers = AccessModifiers.Public)]
    public class InvoiceLine
    {
        public Guid Id { get; set; }

        public Guid ProductId { get; set; }

        public Guid InvoiceId { get; set; }

        public int No { get; set; }

        public int Quantity { get; set; }

        public decimal Amount { get; set; }

        public virtual Product Product { get; set; }
    }
}