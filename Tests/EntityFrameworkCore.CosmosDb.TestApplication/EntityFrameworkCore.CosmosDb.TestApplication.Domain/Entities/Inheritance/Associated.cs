using System;
using System.Collections.Generic;
using EntityFrameworkCore.CosmosDb.TestApplication.Domain.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace EntityFrameworkCore.CosmosDb.TestApplication.Domain.Entities.Inheritance
{
    public class Associated : IHasDomainEvent
    {
        public Associated()
        {
            PartitionKey = null!;
            AssociatedField1 = null!;
        }

        public Guid Id { get; set; }

        public string PartitionKey { get; set; }

        public string AssociatedField1 { get; set; }

        public List<DomainEvent> DomainEvents { get; set; } = [];
    }
}