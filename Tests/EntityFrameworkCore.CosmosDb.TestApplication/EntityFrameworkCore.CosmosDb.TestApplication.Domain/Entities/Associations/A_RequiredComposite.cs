using System;
using System.Collections.Generic;
using EntityFrameworkCore.CosmosDb.TestApplication.Domain.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace EntityFrameworkCore.CosmosDb.TestApplication.Domain.Entities.Associations
{
    public class A_RequiredComposite : IHasDomainEvent
    {
        public A_RequiredComposite()
        {
            RequiredCompositeAttr = null!;
            PartitionKey = null!;
        }

        public Guid Id { get; set; }

        public string RequiredCompositeAttr { get; set; }

        public string PartitionKey { get; set; }

        public virtual A_OptionalDependent? A_OptionalDependent { get; set; }

        public List<DomainEvent> DomainEvents { get; set; } = [];
    }
}