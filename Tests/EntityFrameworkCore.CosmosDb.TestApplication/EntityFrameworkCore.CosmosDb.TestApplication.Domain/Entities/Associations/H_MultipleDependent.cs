using System;
using System.Collections.Generic;
using EntityFrameworkCore.CosmosDb.TestApplication.Domain.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace EntityFrameworkCore.CosmosDb.TestApplication.Domain.Entities.Associations
{
    public class H_MultipleDependent : IHasDomainEvent
    {
        public H_MultipleDependent()
        {
            PartitionKey = null!;
            MultipleDepAttr = null!;
        }

        public Guid Id { get; set; }

        public string PartitionKey { get; set; }

        public string MultipleDepAttr { get; set; }

        public Guid? HOptionalaggregatenavId { get; set; }

        public virtual H_OptionalAggregateNav? H_OptionalAggregateNav { get; set; }

        public List<DomainEvent> DomainEvents { get; set; } = [];
    }
}