using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace CleanArchitecture.TestApplication.Domain.Entities.Inheritance
{
    public class ConcreteClass : BaseClass
    {
        public string ConcreteAttr { get; set; }
    }
}