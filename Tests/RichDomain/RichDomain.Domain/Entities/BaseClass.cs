using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace RichDomain.Domain.Entities
{
    public partial class BaseClass : IBaseClass
    {
        public void BaseOperation(string baseAttribute)
        {
            BaseAttribute = baseAttribute;
        }
    }
}