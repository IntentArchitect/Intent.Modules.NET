using System;
using System.Collections.Generic;
using EntityFrameworkCore.CosmosDb.TestApplication.Domain.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace EntityFrameworkCore.CosmosDb.TestApplication.Domain.Entities.Associations
{
    public class O_DestNameDiff : IHasDomainEvent
    {
        public O_DestNameDiff()
        {
            PartitionKey = null!;
        }
        public Guid Id { get; set; }

        public string PartitionKey { get; set; }

        public virtual ICollection<O_DestNameDiffDependent> DestNameDiff { get; set; } = [];

        public List<DomainEvent> DomainEvents { get; set; } = [];
    }
}