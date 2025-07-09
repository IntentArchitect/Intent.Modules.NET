using System;
using System.Collections.Generic;
using EntityFrameworkCore.CosmosDb.TestApplication.Domain.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace EntityFrameworkCore.CosmosDb.TestApplication.Domain.Entities.InheritanceAssociations
{
    public abstract class AbstractBaseClass : IHasDomainEvent
    {
        public AbstractBaseClass()
        {
            BaseAttribute = null!;
            PartitionKey = null!;
        }

        public Guid Id { get; set; }

        public string BaseAttribute { get; set; }

        public string PartitionKey { get; set; }

        public List<DomainEvent> DomainEvents { get; set; } = [];
    }
}