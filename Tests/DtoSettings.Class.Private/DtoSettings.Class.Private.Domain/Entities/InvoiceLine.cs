using System;
using Intent.RoslynWeaver.Attributes;

namespace DtoSettings.Class.Private.Domain.Entities
{
    public class InvoiceLine
    {
        public Guid Id { get; set; }

        public string Description { get; set; }

        public decimal Amount { get; set; }

        public string Currency { get; set; }

        public Guid InvoiceId { get; set; }
    }
}