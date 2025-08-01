using System;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace MongoDb.TestApplication.Domain.Entities.Indexes
{
    public class CompoundIndexEntitySingleParent
    {
        public CompoundIndexEntitySingleParent()
        {
            Id = null!;
            SomeField = null!;
            CompoundIndexEntitySingleChild = null!;
        }

        public string Id { get; set; }

        public string SomeField { get; set; }

        public CompoundIndexEntitySingleChild CompoundIndexEntitySingleChild { get; set; }
    }
}