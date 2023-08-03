using System;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "1.0")]

namespace CleanArchitecture.TestApplication.Domain.Entities.CRUD
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    [DefaultIntentManaged(Mode.Fully, Targets = Targets.Properties)]
    [DefaultIntentManaged(Mode.Fully, Targets = Targets.Methods, Body = Mode.Ignore, AccessModifiers = AccessModifiers.Public)]
    public class ImplicitKeyNestedComposition
    {
        [IntentManaged(Mode.Fully)]
        public ImplicitKeyNestedComposition()
        {
            Attribute = null!;
        }
        public Guid Id { get; set; }

        public string Attribute { get; set; }

        public Guid ImplicitKeyAggrRootId { get; set; }
    }
}