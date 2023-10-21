using System;
using Intent.RoslynWeaver.Attributes;

namespace Entities.PrivateSetters.TestApplication.Domain.Entities
{
    [DefaultIntentManaged(Mode.Fully, Targets = Targets.Methods, Body = Mode.Ignore, AccessModifiers = AccessModifiers.Public)]
    public class Line
    {
        public Guid Id { get; private set; }
        public string Description { get; private set; }

        public int Quantity { get; private set; }

        public Guid InvoiceId { get; private set; }
    }
}