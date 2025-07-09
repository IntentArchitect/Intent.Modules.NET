using System;
using System.Collections.Generic;
using EntityFrameworkCore.CosmosDb.TestApplication.Domain.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace EntityFrameworkCore.CosmosDb.TestApplication.Domain.Entities.InheritanceAssociations
{
    public class AbstractBaseClassAssociated : IHasDomainEvent
    {
        public AbstractBaseClassAssociated()
        {
            PartitionKey = null!;
            AssociatedField = null!;
            AbstractBaseClass = null!;
        }

        public Guid Id { get; set; }

        public string PartitionKey { get; set; }

        public string AssociatedField { get; set; }

        public Guid AbstractBaseClassId { get; set; }

        public virtual AbstractBaseClass AbstractBaseClass { get; set; }

        public List<DomainEvent> DomainEvents { get; set; } = [];
    }
}