using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "1.0")]

namespace RichDomain.Domain.Entities
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    [DefaultIntentManaged(Mode.Fully, Targets = Targets.Properties)]
    [DefaultIntentManaged(Mode.Fully, Targets = Targets.Methods | Targets.Constructors, Body = Mode.Ignore, AccessModifiers = AccessModifiers.Public)]
    public partial class BaseClass : IBaseClass
    {
        [IntentManaged(Mode.Fully)]
        public BaseClass()
        {
            BaseAttribute = null!;
        }
        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public void BaseOperation(string baseAttribute)
        {
            BaseAttribute = baseAttribute;
        }
    }
}