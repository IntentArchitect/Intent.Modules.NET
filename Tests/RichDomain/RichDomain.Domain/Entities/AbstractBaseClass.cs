using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace RichDomain.Domain.Entities
{
    public abstract partial class AbstractBaseClass : IAbstractBaseClass
    {
        public void BaseOperation(string abstractBaseAttribute)
        {
            AbstractBaseAttribute = abstractBaseAttribute;
        }

        public abstract bool AbstractOp(string thing);
    }
}