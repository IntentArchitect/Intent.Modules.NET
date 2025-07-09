using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace GraphQL.CQRS.TestApplication.Domain.Entities
{
    public class Invoice
    {
        public Invoice()
        {
            Customer = null!;
        }

        public Guid Id { get; set; }

        public int No { get; set; }

        public DateTime Created { get; set; }

        public Guid CustomerId { get; set; }

        public virtual ICollection<InvoiceLine> InvoiceLines { get; set; } = [];

        public virtual Customer Customer { get; set; }
    }
}