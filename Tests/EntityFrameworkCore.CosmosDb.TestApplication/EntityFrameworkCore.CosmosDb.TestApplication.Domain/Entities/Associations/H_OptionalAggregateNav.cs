using System;
using System.Collections.Generic;
using EntityFrameworkCore.CosmosDb.TestApplication.Domain.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace EntityFrameworkCore.CosmosDb.TestApplication.Domain.Entities.Associations
{
    public class H_OptionalAggregateNav : IHasDomainEvent
    {
        public H_OptionalAggregateNav()
        {
            PartitionKey = null!;
            OptionalAggrNavAttr = null!;
        }

        public Guid Id { get; set; }

        public string PartitionKey { get; set; }

        public string OptionalAggrNavAttr { get; set; }

        public virtual ICollection<H_MultipleDependent> H_MultipleDependents { get; set; } = [];

        public List<DomainEvent> DomainEvents { get; set; } = [];
    }
}