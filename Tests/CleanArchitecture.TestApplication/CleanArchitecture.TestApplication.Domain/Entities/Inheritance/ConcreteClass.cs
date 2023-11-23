using Intent.RoslynWeaver.Attributes;

[assembly: IntentTagModeImplicit]

namespace CleanArchitecture.TestApplication.Domain.Entities.Inheritance
{
    public class ConcreteClass : BaseClass
    {
        public string ConcreteAttr { get; set; }
    }
}