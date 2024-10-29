using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DataContract", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Domain.Contracts.DataContracts
{
    public record BaseDataContract
    {
        public BaseDataContract(string attributeOnBase)
        {
            AttributeOnBase = attributeOnBase;
        }

        public string AttributeOnBase { get; init; }
    }
}