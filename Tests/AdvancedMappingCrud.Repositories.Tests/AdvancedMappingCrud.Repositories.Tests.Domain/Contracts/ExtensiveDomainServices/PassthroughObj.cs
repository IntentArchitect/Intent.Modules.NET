using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DataContract", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Domain.Contracts.ExtensiveDomainServices
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