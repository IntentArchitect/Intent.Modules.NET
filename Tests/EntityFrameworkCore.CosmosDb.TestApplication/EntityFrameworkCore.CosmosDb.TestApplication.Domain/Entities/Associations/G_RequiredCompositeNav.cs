using System;
using System.Collections.Generic;
using EntityFrameworkCore.CosmosDb.TestApplication.Domain.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace EntityFrameworkCore.CosmosDb.TestApplication.Domain.Entities.Associations
{
    public class G_RequiredCompositeNav : IHasDomainEvent
    {
        public G_RequiredCompositeNav()
        {
            PartitionKey = null!;
            RequiredCompNavAttr = null!;
        }

        public Guid Id { get; set; }

        public string PartitionKey { get; set; }

        public string RequiredCompNavAttr { get; set; }

        public virtual ICollection<G_MultipleDependent> G_MultipleDependents { get; set; } = [];

        public List<DomainEvent> DomainEvents { get; set; } = [];
    }
}