using System;
using System.Collections.Generic;
using EntityFrameworkCore.CosmosDb.TestApplication.Domain.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace EntityFrameworkCore.CosmosDb.TestApplication.Domain.Entities.Inheritance
{
    public class BaseAssociated : IHasDomainEvent
    {
        public BaseAssociated()
        {
            PartitionKey = null!;
            BaseAssociatedField1 = null!;
        }

        public Guid Id { get; set; }

        public string PartitionKey { get; set; }

        public string BaseAssociatedField1 { get; set; }

        public List<DomainEvent> DomainEvents { get; set; } = [];
    }
}