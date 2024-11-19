using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace CleanArchitecture.Comprehensive.Domain.Entities.Inheritance
{
    public class ConcreteClass : BaseClass
    {
        public ConcreteClass()
        {
            ConcreteAttr = null!;
        }
        public string ConcreteAttr { get; set; }
    }
}