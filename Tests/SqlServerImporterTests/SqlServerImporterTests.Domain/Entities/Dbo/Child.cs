using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;
using SqlServerImporterTests.Domain.Common;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace SqlServerImporterTests.Domain.Entities.Dbo
{
    public class Child : IHasDomainEvent
    {
        public Guid Id { get; set; }

        public Guid ParentId { get; set; }

        public Guid ParentId2 { get; set; }

        public virtual Parent Parent { get; set; }

        public List<DomainEvent> DomainEvents { get; set; } = new List<DomainEvent>();
    }
}