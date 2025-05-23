using System;
using System.Collections.Generic;
using EntityFrameworkCore.CosmosDb.TestApplication.Domain.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace EntityFrameworkCore.CosmosDb.TestApplication.Domain.Entities.Associations
{
    public class P_SourceNameDiff : IHasDomainEvent
    {
        public P_SourceNameDiff()
        {
            PartitionKey = null!;
        }
        public Guid Id { get; set; }

        public string PartitionKey { get; set; }

        public virtual ICollection<P_SourceNameDiffDependent> P_SourceNameDiffDependents { get; set; } = [];

        public List<DomainEvent> DomainEvents { get; set; } = [];
    }
}