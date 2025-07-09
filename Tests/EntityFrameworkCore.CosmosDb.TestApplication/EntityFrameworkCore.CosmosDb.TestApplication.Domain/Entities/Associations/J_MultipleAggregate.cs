using System;
using System.Collections.Generic;
using EntityFrameworkCore.CosmosDb.TestApplication.Domain.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace EntityFrameworkCore.CosmosDb.TestApplication.Domain.Entities.Associations
{
    public class J_MultipleAggregate : IHasDomainEvent
    {
        public J_MultipleAggregate()
        {
            PartitionKey = null!;
            MultipleAggrAttr = null!;
            J_RequiredDependent = null!;
        }

        public Guid Id { get; set; }

        public string PartitionKey { get; set; }

        public string MultipleAggrAttr { get; set; }

        public Guid JRequireddependentId { get; set; }

        public virtual J_RequiredDependent J_RequiredDependent { get; set; }

        public List<DomainEvent> DomainEvents { get; set; } = [];
    }
}