using System;
using System.Collections.Generic;
using EntityFrameworkCore.CosmosDb.TestApplication.Domain.Common;
using Intent.RoslynWeaver.Attributes;

namespace EntityFrameworkCore.CosmosDb.TestApplication.Domain.Entities.Inheritance
{
    [DefaultIntentManaged(Mode.Fully, Targets = Targets.Methods, Body = Mode.Ignore, AccessModifiers = AccessModifiers.Public)]
    public class Base : IHasDomainEvent
    {
        public Guid Id { get; set; }

        public string BaseField1 { get; set; }

        public string PartitionKey { get; set; }

        public Guid? BaseAssociatedId { get; set; }

        public virtual BaseAssociated? BaseAssociated { get; set; }

        public List<DomainEvent> DomainEvents { get; set; } = new List<DomainEvent>();
    }
}