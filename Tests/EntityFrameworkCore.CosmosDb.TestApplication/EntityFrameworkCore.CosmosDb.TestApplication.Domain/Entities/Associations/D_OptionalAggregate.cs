using System;
using System.Collections.Generic;
using EntityFrameworkCore.CosmosDb.TestApplication.Domain.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace EntityFrameworkCore.CosmosDb.TestApplication.Domain.Entities.Associations
{
    public class D_OptionalAggregate : IHasDomainEvent
    {
        public D_OptionalAggregate()
        {
            PartitionKey = null!;
            OptionalAggregateAttr = null!;
        }

        public Guid Id { get; set; }

        public string PartitionKey { get; set; }

        public string OptionalAggregateAttr { get; set; }

        public virtual ICollection<D_MultipleDependent> D_MultipleDependents { get; set; } = [];

        public List<DomainEvent> DomainEvents { get; set; } = [];
    }
}