using System;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace GraphQL.CQRS.TestApplication.Domain.Entities
{
    public class InvoiceLine
    {
        public InvoiceLine()
        {
            Product = null!;
        }

        public Guid ProductId { get; set; }

        public Guid InvoiceId { get; set; }

        public int No { get; set; }

        public int Quantity { get; set; }

        public decimal Amount { get; set; }

        public virtual Product Product { get; set; }
    }
}