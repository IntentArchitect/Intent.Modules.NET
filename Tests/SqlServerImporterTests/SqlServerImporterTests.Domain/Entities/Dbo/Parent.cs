using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;
using SqlServerImporterTests.Domain.Common;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace SqlServerImporterTests.Domain.Entities.Dbo
{
    public class Parent : IHasDomainEvent
    {
        public Guid Id { get; set; }

        public Guid Id2 { get; set; }

        public string Name { get; set; }

        public List<DomainEvent> DomainEvents { get; set; } = new List<DomainEvent>();
    }
}