using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace Standard.AspNetCore.TestApplication.Domain.Entities
{
    public class Invoice
    {
        public Invoice()
        {
            Number = null!;
        }
        public Guid Id { get; set; }

        public string Number { get; set; }

        public virtual ICollection<InvoiceLine> InvoiceLines { get; set; } = [];
    }
}