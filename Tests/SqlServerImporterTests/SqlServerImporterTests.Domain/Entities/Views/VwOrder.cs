using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;
using SqlServerImporterTests.Domain.Common;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace SqlServerImporterTests.Domain.Entities.Views
{
    public class VwOrder : IHasDomainEvent
    {
        public Guid Id { get; set; }

        public Guid CustomerId { get; set; }

        public DateTime OrderDate { get; set; }

        public string RefNo { get; set; }

        public List<DomainEvent> DomainEvents { get; set; } = new List<DomainEvent>();
    }
}