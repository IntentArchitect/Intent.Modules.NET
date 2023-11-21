using System;
using System.Collections.Generic;
using EntityFrameworkCore.CosmosDb.TestApplication.Domain.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTagModeImplicit]

namespace EntityFrameworkCore.CosmosDb.TestApplication.Domain.Entities.InheritanceAssociations
{
    [DefaultIntentManaged(Mode.Fully, Targets = Targets.Methods, Body = Mode.Ignore, AccessModifiers = AccessModifiers.Public)]
    public abstract class AbstractBaseClass : IHasDomainEvent
    {
        public Guid Id { get; set; }

        public string BaseAttribute { get; set; }

        public string PartitionKey { get; set; }

        public List<DomainEvent> DomainEvents { get; set; } = new List<DomainEvent>();
    }
}