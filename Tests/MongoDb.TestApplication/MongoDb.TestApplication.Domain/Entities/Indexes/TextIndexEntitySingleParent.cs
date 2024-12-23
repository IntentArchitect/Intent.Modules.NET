using System;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace MongoDb.TestApplication.Domain.Entities.Indexes
{
    [DefaultIntentManaged(Mode.Fully, Targets = Targets.Methods, Body = Mode.Ignore, AccessModifiers = AccessModifiers.Public)]
    public class TextIndexEntitySingleParent
    {
        public TextIndexEntitySingleParent()
        {
            Id = null!;
            SomeField = null!;
            TextIndexEntitySingleChild = null!;
        }
        public string Id { get; set; }

        public string SomeField { get; set; }

        public TextIndexEntitySingleChild TextIndexEntitySingleChild { get; set; }
    }
}