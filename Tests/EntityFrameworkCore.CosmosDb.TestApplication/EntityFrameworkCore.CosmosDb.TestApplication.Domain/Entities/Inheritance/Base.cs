using System;
using System.Collections.Generic;
using EntityFrameworkCore.CosmosDb.TestApplication.Domain.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace EntityFrameworkCore.CosmosDb.TestApplication.Domain.Entities.Inheritance
{
    [DefaultIntentManaged(Mode.Fully, Targets = Targets.Methods, Body = Mode.Ignore, AccessModifiers = AccessModifiers.Public)]
    public class Base : IHasDomainEvent
    {
        public Base()
        {
            BaseField1 = null!;
            PartitionKey = null!;
        }
        public Guid Id { get; set; }

        public string BaseField1 { get; set; }

        public string PartitionKey { get; set; }

        public Guid? BaseAssociatedId { get; set; }

        public virtual BaseAssociated? BaseAssociated { get; set; }

        public List<DomainEvent> DomainEvents { get; set; } = [];
    }
}