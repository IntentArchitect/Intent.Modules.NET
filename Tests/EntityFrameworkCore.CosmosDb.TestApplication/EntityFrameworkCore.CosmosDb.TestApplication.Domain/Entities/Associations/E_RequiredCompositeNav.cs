using System;
using System.Collections.Generic;
using EntityFrameworkCore.CosmosDb.TestApplication.Domain.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace EntityFrameworkCore.CosmosDb.TestApplication.Domain.Entities.Associations
{
    public class E_RequiredCompositeNav : IHasDomainEvent
    {
        public E_RequiredCompositeNav()
        {
            RequiredCompositeNavAttr = null!;
            PartitionKey = null!;
            E_RequiredDependent = null!;
        }

        public Guid Id { get; set; }

        public string RequiredCompositeNavAttr { get; set; }

        public string PartitionKey { get; set; }

        public virtual E_RequiredDependent E_RequiredDependent { get; set; }

        public List<DomainEvent> DomainEvents { get; set; } = [];
    }
}