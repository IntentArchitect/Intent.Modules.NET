using System;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTagModeImplicit]

namespace RichDomain.Domain.Entities
{
    public partial class DerivedFromAbstractClass : AbstractBaseClass, IDerivedFromAbstractClass
    {
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