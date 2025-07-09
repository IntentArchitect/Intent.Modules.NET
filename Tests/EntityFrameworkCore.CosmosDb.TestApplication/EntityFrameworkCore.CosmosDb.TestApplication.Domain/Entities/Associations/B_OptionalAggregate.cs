using System;
using System.Collections.Generic;
using EntityFrameworkCore.CosmosDb.TestApplication.Domain.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace EntityFrameworkCore.CosmosDb.TestApplication.Domain.Entities.Associations
{
    public class B_OptionalAggregate : IHasDomainEvent
    {
        public B_OptionalAggregate()
        {
            OptionalAggregateAttr = null!;
            PartitionKey = null!;
        }

        public Guid Id { get; set; }

        public string OptionalAggregateAttr { get; set; }

        public string PartitionKey { get; set; }

        public Guid? B_OptionalDependentId { get; set; }

        public virtual B_OptionalDependent? B_OptionalDependent { get; set; }

        public List<DomainEvent> DomainEvents { get; set; } = [];
    }
}