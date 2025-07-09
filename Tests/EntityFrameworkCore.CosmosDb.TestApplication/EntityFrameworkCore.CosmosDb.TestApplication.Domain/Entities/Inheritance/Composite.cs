using System;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace EntityFrameworkCore.CosmosDb.TestApplication.Domain.Entities.Inheritance
{
    public class Composite
    {
        public Composite()
        {
            CompositeField1 = null!;
            PartitionKey = null!;
        }

        public Guid Id { get; set; }

        public string CompositeField1 { get; set; }

        public string PartitionKey { get; set; }

        public Guid DerivedId { get; set; }
    }
}