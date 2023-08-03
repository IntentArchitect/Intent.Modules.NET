using System;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "1.0")]

namespace CleanArchitecture.TestApplication.Domain.Entities.CRUD
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    [DefaultIntentManaged(Mode.Fully, Targets = Targets.Properties)]
    [DefaultIntentManaged(Mode.Fully, Targets = Targets.Methods, Body = Mode.Ignore, AccessModifiers = AccessModifiers.Public)]
    public class CompositeManyAA
    {
        [IntentManaged(Mode.Fully)]
        public CompositeManyAA()
        {
            CompositeAttr = null!;
        }
        public Guid Id { get; set; }

        public string CompositeAttr { get; set; }

        public Guid CompositeSingleAId { get; set; }
    }
}