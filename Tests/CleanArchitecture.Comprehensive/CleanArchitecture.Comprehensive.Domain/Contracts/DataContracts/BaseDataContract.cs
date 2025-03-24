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

        /// <summary>
        /// Required by Entity Framework.
        /// </summary>
        [IntentManaged(Mode.Fully)]
        protected BaseDataContract()
        {
            AttributeOnBase = null!;
        }

        public string AttributeOnBase { get; init; }
    }
}