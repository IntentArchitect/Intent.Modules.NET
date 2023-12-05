using System;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace Standard.AspNetCore.TestApplication.Domain.Entities
{
    public class InvoiceLine
    {
        public Guid Id { get; set; }

        public string Description { get; set; }

        public Guid InvoiceId { get; set; }
    }
}