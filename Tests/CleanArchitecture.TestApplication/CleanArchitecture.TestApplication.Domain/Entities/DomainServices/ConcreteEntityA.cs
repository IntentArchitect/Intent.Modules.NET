using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace CleanArchitecture.TestApplication.Domain.Entities.DomainServices
{
    public class ConcreteEntityA : BaseEntityA
    {
        public string ConcreteAttr { get; set; }
    }
}