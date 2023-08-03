using System;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "1.0")]

namespace MongoDb.TestApplication.Domain.Entities.NestedAssociations
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    [DefaultIntentManaged(Mode.Fully, Targets = Targets.Properties)]
    [DefaultIntentManaged(Mode.Fully, Targets = Targets.Methods, Body = Mode.Ignore, AccessModifiers = AccessModifiers.Public)]
    public class AggregateA
    {
        [IntentManaged(Mode.Fully)]
        public AggregateA()
        {
            Id = null!;
            Attribute = null!;
            NestedCompositionA = null!;
        }
        public string Id { get; set; }

        public string Attribute { get; set; }

        public virtual NestedCompositionA NestedCompositionA { get; set; }
    }
}