using System;
using System.Collections.Generic;
using EntityFrameworkCore.CosmosDb.TestApplication.Domain.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace EntityFrameworkCore.CosmosDb.TestApplication.Domain.Entities.InheritanceAssociations
{
    [DefaultIntentManaged(Mode.Fully, Targets = Targets.Methods, Body = Mode.Ignore, AccessModifiers = AccessModifiers.Public)]
    public abstract class StandaloneAbstract : IHasDomainEvent
    {
        public StandaloneAbstract()
        {
            PartitionKey = null!;
            BaseAttribute = null!;
        }
        public Guid Id { get; set; }

        public string PartitionKey { get; set; }

        public string BaseAttribute { get; set; }

        public List<DomainEvent> DomainEvents { get; set; } = [];
    }
}