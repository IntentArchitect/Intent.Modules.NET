using System;
using System.Collections.Generic;
using EntityFrameworkCore.CosmosDb.TestApplication.Domain.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace EntityFrameworkCore.CosmosDb.TestApplication.Domain.Entities.Associations
{
    public class S_NoPkInComposite : IHasDomainEvent
    {
        public S_NoPkInComposite()
        {
            PartitionKey = null!;
            Description = null!;
            S_NoPkInCompositeDependent = null!;
        }
        public Guid Id { get; set; }

        public string PartitionKey { get; set; }

        public string Description { get; set; }

        public virtual S_NoPkInCompositeDependent S_NoPkInCompositeDependent { get; set; }

        public List<DomainEvent> DomainEvents { get; set; } = [];
    }
}