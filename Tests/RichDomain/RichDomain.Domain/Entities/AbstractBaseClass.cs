using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "1.0")]

namespace RichDomain.Domain.Entities
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    [DefaultIntentManaged(Mode.Fully, Targets = Targets.Properties)]
    [DefaultIntentManaged(Mode.Fully, Targets = Targets.Methods | Targets.Constructors, Body = Mode.Ignore, AccessModifiers = AccessModifiers.Public)]
    public abstract partial class AbstractBaseClass : IAbstractBaseClass
    {
        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public void BaseOperation(string abstractBaseAttribute)
        {
            AbstractBaseAttribute = abstractBaseAttribute;
        }

        public abstract bool AbstractOp(string thing);

        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public virtual void VirtualOp(string abstractBaseAttribute)
        {
            AbstractBaseAttribute = abstractBaseAttribute;
        }
    }
}