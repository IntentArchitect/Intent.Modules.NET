using System;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "1.0")]

namespace Standard.AspNetCore.TestApplication.Domain.Entities
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    [DefaultIntentManaged(Mode.Fully, Targets = Targets.Properties)]
    [DefaultIntentManaged(Mode.Fully, Targets = Targets.Methods | Targets.Constructors, Body = Mode.Ignore, AccessModifiers = AccessModifiers.Public)]
    public class InvoiceLine
    {
        [IntentManaged(Mode.Fully)]
        public InvoiceLine()
        {
            Description = null!;
        }
        public Guid Id { get; set; }

        public string Description { get; set; }

        public Guid InvoiceId { get; set; }
    }
}