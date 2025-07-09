using System;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace MongoDb.TestApplication.Domain.Entities.Indexes
{
    public class SingleIndexEntitySingleParent
    {
        public SingleIndexEntitySingleParent()
        {
            Id = null!;
            SomeField = null!;
            SingleIndexEntitySingleChild = null!;
        }

        public string Id { get; set; }

        public string SomeField { get; set; }

        public SingleIndexEntitySingleChild SingleIndexEntitySingleChild { get; set; }
    }
}