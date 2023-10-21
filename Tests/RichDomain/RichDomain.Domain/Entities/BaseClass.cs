using Intent.RoslynWeaver.Attributes;

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