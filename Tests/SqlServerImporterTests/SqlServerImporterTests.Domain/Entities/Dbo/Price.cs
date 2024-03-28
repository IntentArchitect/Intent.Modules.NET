using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;
using SqlServerImporterTests.Domain.Common;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace SqlServerImporterTests.Domain.Entities.Dbo
{
    public class Price : IHasDomainEvent
    {
        public Guid Id { get; set; }

        public Guid ProductId { get; set; }

        public DateTime ActiveFrom { get; set; }

        public decimal Amount { get; set; }

        public virtual Product Product { get; set; }

        public List<DomainEvent> DomainEvents { get; set; } = new List<DomainEvent>();
    }
}