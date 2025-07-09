using System;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace MongoDb.TestApplication.Domain.Entities.Indexes
{
    public class MultikeyIndexEntitySingleParent
    {
        public MultikeyIndexEntitySingleParent()
        {
            Id = null!;
            SomeField = null!;
            MultikeyIndexEntitySingleChild = null!;
        }

        public string Id { get; set; }

        public string SomeField { get; set; }

        public MultikeyIndexEntitySingleChild MultikeyIndexEntitySingleChild { get; set; }
    }
}