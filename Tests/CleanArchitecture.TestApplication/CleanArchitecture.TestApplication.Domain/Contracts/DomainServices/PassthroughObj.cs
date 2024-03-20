using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DataContract", Version = "1.0")]

namespace CleanArchitecture.TestApplication.Domain.Contracts.DomainServices
{
    public record PassthroughObj : PassthroughBaseObj
    {
        public PassthroughObj(string baseAttr, string concreteAttr) : base(baseAttr)
        {
            ConcreteAttr = concreteAttr;
        }

        public string ConcreteAttr { get; init; }
    }
}