using System;
using System.Collections.Generic;
using EntityFrameworkCore.SqlServer.TestApplication.Domain.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTagModeImplicit]

namespace EntityFrameworkCore.SqlServer.TestApplication.Domain.Entities
{
    public class SchemaParent : IHasDomainEvent
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public virtual SchemaInLineChild SchemaInLineChild { get; set; }

        public List<DomainEvent> DomainEvents { get; set; } = new List<DomainEvent>();
    }
}