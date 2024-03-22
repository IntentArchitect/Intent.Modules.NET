using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DataContract", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Domain.Contracts.ExtensiveDomainServices
{
    public record PassthroughBaseObj
    {
        public PassthroughBaseObj(string baseAttr)
        {
            BaseAttr = baseAttr;
        }

        public string BaseAttr { get; init; }
    }
}