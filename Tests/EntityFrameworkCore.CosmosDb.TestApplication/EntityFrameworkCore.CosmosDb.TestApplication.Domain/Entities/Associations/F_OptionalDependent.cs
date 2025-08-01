using System;
using System.Collections.Generic;
using EntityFrameworkCore.CosmosDb.TestApplication.Domain.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace EntityFrameworkCore.CosmosDb.TestApplication.Domain.Entities.Associations
{
    public class F_OptionalDependent : IHasDomainEvent
    {
        public F_OptionalDependent()
        {
            PartitionKey = null!;
            OptionalDependentAttr = null!;
        }

        public Guid Id { get; set; }

        public string PartitionKey { get; set; }

        public string OptionalDependentAttr { get; set; }

        public virtual F_OptionalAggregateNav? F_OptionalAggregateNav { get; set; }

        public List<DomainEvent> DomainEvents { get; set; } = [];
    }
}