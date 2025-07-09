using System;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace Entities.PrivateSetters.TestApplication.Domain.Entities
{
    public class Line
    {
        public Line()
        {
            Description = null!;
        }

        public Guid Id { get; private set; }

        public string Description { get; private set; }

        public int Quantity { get; private set; }

        public Guid InvoiceId { get; private set; }
    }
}