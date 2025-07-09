using System;
using System.Collections.Generic;
using EntityFrameworkCore.CosmosDb.TestApplication.Domain.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace EntityFrameworkCore.CosmosDb.TestApplication.Domain.Entities.Associations
{
    public class C_RequiredComposite : IHasDomainEvent
    {
        public C_RequiredComposite()
        {
            PartitionKey = null!;
            RequiredCompositeAttr = null!;
        }

        public Guid Id { get; set; }

        public string PartitionKey { get; set; }

        public string RequiredCompositeAttr { get; set; }

        public virtual ICollection<C_MultipleDependent> C_MultipleDependents { get; set; } = [];

        public List<DomainEvent> DomainEvents { get; set; } = [];
    }
}