using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace AdvancedMappingCrud.RichDomain.Tests.Domain.Entities
{
    public class SpecializedProduct : Product
    {
        public SpecializedProduct()
        {
            Code = null!;
        }

        public string Code { get; private set; }
    }
}