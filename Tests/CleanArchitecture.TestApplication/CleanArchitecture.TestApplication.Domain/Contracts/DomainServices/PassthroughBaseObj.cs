using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DataContract", Version = "1.0")]

namespace CleanArchitecture.TestApplication.Domain.Contracts.DomainServices
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