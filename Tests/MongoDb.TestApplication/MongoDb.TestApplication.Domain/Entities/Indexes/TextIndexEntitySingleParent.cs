using System;
using Intent.RoslynWeaver.Attributes;

namespace MongoDb.TestApplication.Domain.Entities.Indexes
{
    [DefaultIntentManaged(Mode.Fully, Targets = Targets.Methods, Body = Mode.Ignore, AccessModifiers = AccessModifiers.Public)]
    public class TextIndexEntitySingleParent
    {
        public string Id { get; set; }

        public string SomeField { get; set; }

        public TextIndexEntitySingleChild TextIndexEntitySingleChild { get; set; }
    }
}