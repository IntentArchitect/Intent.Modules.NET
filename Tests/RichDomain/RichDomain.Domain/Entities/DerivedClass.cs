using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

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