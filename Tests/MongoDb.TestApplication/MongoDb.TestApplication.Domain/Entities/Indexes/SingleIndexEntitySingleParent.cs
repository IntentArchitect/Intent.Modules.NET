using System;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTagModeImplicit]

namespace MongoDb.TestApplication.Domain.Entities.Indexes
{
    [DefaultIntentManaged(Mode.Fully, Targets = Targets.Methods, Body = Mode.Ignore, AccessModifiers = AccessModifiers.Public)]
    public class SingleIndexEntitySingleParent
    {
        public string Id { get; set; }

        public string SomeField { get; set; }

        public SingleIndexEntitySingleChild SingleIndexEntitySingleChild { get; set; }
    }
}