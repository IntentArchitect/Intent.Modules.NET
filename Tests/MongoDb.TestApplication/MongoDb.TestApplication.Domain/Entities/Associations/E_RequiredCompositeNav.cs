using System;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "1.0")]

namespace MongoDb.TestApplication.Domain.Entities.Associations
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    [DefaultIntentManaged(Mode.Fully, Targets = Targets.Properties)]
    [DefaultIntentManaged(Mode.Fully, Targets = Targets.Methods, Body = Mode.Ignore, AccessModifiers = AccessModifiers.Public)]
    public class E_RequiredCompositeNav
    {
        [IntentManaged(Mode.Fully)]
        public E_RequiredCompositeNav()
        {
            Id = null!;
            Attribute = null!;
            E_RequiredDependent = null!;
        }
        public string Id { get; set; }

        public string Attribute { get; set; }

        public virtual E_RequiredDependent E_RequiredDependent { get; set; }
    }
}