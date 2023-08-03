using System;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "1.0")]

namespace MongoDb.TestApplication.Domain.Entities.NestedAssociations
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    [DefaultIntentManaged(Mode.Fully, Targets = Targets.Properties)]
    [DefaultIntentManaged(Mode.Fully, Targets = Targets.Methods, Body = Mode.Ignore, AccessModifiers = AccessModifiers.Public)]
    public class NestedCompositionA
    {
        [IntentManaged(Mode.Fully)]
        public NestedCompositionA()
        {
            Attribute = null!;
            AggregateBId = null!;
            AggregateB = null!;
        }
        public string Attribute { get; set; }

        public string AggregateBId { get; set; }
    }
}