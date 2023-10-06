using Intent.RoslynWeaver.Attributes;

namespace RichDomain.Domain.Entities
{
    public partial class DerivedClass : BaseClass, IDerivedClass
    {
        public void DerivedOperation(string derivedAttribute, string baseClassBaseAttribute)
        {
            DerivedAttribute = derivedAttribute;
            BaseAttribute = baseClassBaseAttribute;
        }
    }
}