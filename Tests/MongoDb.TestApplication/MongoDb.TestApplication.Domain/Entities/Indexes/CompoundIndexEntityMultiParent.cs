using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTagModeImplicit]

namespace MongoDb.TestApplication.Domain.Entities.Indexes
{
    [DefaultIntentManaged(Mode.Fully, Targets = Targets.Methods, Body = Mode.Ignore, AccessModifiers = AccessModifiers.Public)]
    public class CompoundIndexEntityMultiParent
    {
        public string Id { get; set; }

        public string SomeField { get; set; }

        public ICollection<CompoundIndexEntityMultiChild> CompoundIndexEntityMultiChild { get; set; } = new List<CompoundIndexEntityMultiChild>();
    }
}