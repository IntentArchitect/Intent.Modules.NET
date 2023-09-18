using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "1.0")]

namespace MongoDb.TestApplication.Domain.Entities.Indexes
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    [DefaultIntentManaged(Mode.Fully, Targets = Targets.Properties)]
    [DefaultIntentManaged(Mode.Fully, Targets = Targets.Methods, Body = Mode.Ignore, AccessModifiers = AccessModifiers.Public)]
    public class CompoundIndexEntityMultiParent
    {
        public string Id { get; set; }

        public string SomeField { get; set; }

        public ICollection<CompoundIndexEntityMultiChild> CompoundIndexEntityMultiChild { get; set; } = new List<CompoundIndexEntityMultiChild>();
    }
}