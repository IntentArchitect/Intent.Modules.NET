using System;
using System.Collections.Generic;
using EntityFrameworkCore.CosmosDb.TestApplication.Domain.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace EntityFrameworkCore.CosmosDb.TestApplication.Domain.Entities.Associations
{
    public class R_SourceNameDiff : IHasDomainEvent
    {
        public R_SourceNameDiff()
        {
            PartitionKey = null!;
            R_SourceNameDiffDependent = null!;
        }
        public Guid Id { get; set; }

        public string PartitionKey { get; set; }

        public virtual R_SourceNameDiffDependent R_SourceNameDiffDependent { get; set; }

        public List<DomainEvent> DomainEvents { get; set; } = [];
    }
}