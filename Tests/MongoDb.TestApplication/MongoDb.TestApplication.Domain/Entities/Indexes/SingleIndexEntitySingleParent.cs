using System;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace MongoDb.TestApplication.Domain.Entities.Indexes
{
    [DefaultIntentManaged(Mode.Fully, Targets = Targets.Methods, Body = Mode.Ignore, AccessModifiers = AccessModifiers.Public)]
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