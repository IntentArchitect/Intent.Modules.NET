using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "1.0")]

namespace CqrsAutoCrud.TestApplication.Domain.Entities
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    [DefaultIntentManaged(Mode.Fully, Targets = Targets.Properties)]
    [DefaultIntentManaged(Mode.Fully, Targets = Targets.Methods, Body = Mode.Ignore, AccessModifiers = AccessModifiers.Public)]
    public class ImplicitKeyAggrRoot
    {
        public Guid Id { get; set; }

        public string Attribute { get; set; }

        public virtual ICollection<ImplicitKeyNestedComposition> ImplicitKeyNestedCompositions { get; set; } = new List<ImplicitKeyNestedComposition>();
    }
}