using System;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace MongoDb.TestApplication.Domain.Entities.Indexes
{
    [DefaultIntentManaged(Mode.Fully, Targets = Targets.Methods, Body = Mode.Ignore, AccessModifiers = AccessModifiers.Public)]
    public class SingleIndexEntitySingleChild
    {
        public SingleIndexEntitySingleChild()
        {
            SingleIndex = null!;
        }
        private string? _id;

        public string SingleIndex { get; set; }
    }
}