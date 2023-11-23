using Intent.RoslynWeaver.Attributes;

[assembly: IntentTagModeImplicit]

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