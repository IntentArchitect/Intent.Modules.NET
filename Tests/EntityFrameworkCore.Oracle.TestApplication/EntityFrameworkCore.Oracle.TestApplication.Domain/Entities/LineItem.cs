using System;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace EntityFrameworkCore.Oracle.TestApplication.Domain.Entities
{
    public class LineItem
    {
        public Guid Id { get; set; }

        public string Description { get; set; }

        public Guid InvoiceId { get; set; }
    }
}