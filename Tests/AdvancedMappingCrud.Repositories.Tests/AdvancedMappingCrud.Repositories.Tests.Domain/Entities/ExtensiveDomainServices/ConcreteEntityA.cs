using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Domain.Entities.ExtensiveDomainServices
{
    public class ConcreteEntityA : BaseEntityA
    {
        public ConcreteEntityA()
        {
            ConcreteAttr = null!;
        }
        public string ConcreteAttr { get; set; }
    }
}