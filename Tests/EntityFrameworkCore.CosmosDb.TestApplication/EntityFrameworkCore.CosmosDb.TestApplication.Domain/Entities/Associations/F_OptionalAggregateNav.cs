using System;
using System.Collections.Generic;
using EntityFrameworkCore.CosmosDb.TestApplication.Domain.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace EntityFrameworkCore.CosmosDb.TestApplication.Domain.Entities.Associations
{
    public class F_OptionalAggregateNav : IHasDomainEvent
    {
        public F_OptionalAggregateNav()
        {
            PartitionKey = null!;
            OptionalAggrNavAttr = null!;
        }

        public Guid Id { get; set; }

        public string PartitionKey { get; set; }

        public string OptionalAggrNavAttr { get; set; }

        public Guid? F_OptionalDependentId { get; set; }

        public virtual F_OptionalDependent? F_OptionalDependent { get; set; }

        public List<DomainEvent> DomainEvents { get; set; } = [];
    }
}