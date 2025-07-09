using System;
using System.Collections.Generic;
using EntityFrameworkCore.CosmosDb.TestApplication.Domain.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace EntityFrameworkCore.CosmosDb.TestApplication.Domain.Entities.NestedComposition
{
    public class ClassA : IHasDomainEvent
    {
        public ClassA()
        {
            PartitionKey = null!;
            ClassAAttr = null!;
        }

        public Guid Id { get; set; }

        public string PartitionKey { get; set; }

        public string ClassAAttr { get; set; }

        public virtual ICollection<ClassB> ClassBS { get; set; } = [];

        public List<DomainEvent> DomainEvents { get; set; } = [];
    }
}