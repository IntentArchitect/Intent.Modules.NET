using System;
using System.Collections.Generic;
using EntityFrameworkCore.CosmosDb.TestApplication.Domain.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace EntityFrameworkCore.CosmosDb.TestApplication.Domain.Entities.Associations
{
    public class K_SelfReference : IHasDomainEvent
    {
        public K_SelfReference()
        {
            PartitionKey = null!;
            SelfRefAttr = null!;
        }

        public Guid Id { get; set; }

        public string PartitionKey { get; set; }

        public string SelfRefAttr { get; set; }

        public Guid? KSelfreferencesId { get; set; }

        public virtual K_SelfReference? K_SelfReferenceAssociation { get; set; }

        public List<DomainEvent> DomainEvents { get; set; } = [];
    }
}