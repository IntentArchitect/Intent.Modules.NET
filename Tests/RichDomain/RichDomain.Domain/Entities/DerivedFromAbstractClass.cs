using System;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "1.0")]

namespace RichDomain.Domain.Entities
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    [DefaultIntentManaged(Mode.Fully, Targets = Targets.Properties)]
    [DefaultIntentManaged(Mode.Fully, Targets = Targets.Methods | Targets.Constructors, Body = Mode.Ignore, AccessModifiers = AccessModifiers.Public)]
    public partial class DerivedFromAbstractClass : AbstractBaseClass, IDerivedFromAbstractClass
    {
        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public void DerivedOperation(string derivedAttribute, string abstractBaseClassAbstractBaseAttribute)
        {
            DerivedAttribute = derivedAttribute;
            AbstractBaseAttribute = abstractBaseClassAbstractBaseAttribute;
        }

        public override bool AbstractOp(string thing)
        {
            throw new NotImplementedException("Replace with your implementation...");
        }
    }
}