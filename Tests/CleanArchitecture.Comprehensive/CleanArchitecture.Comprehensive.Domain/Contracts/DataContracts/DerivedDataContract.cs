using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DataContract", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Domain.Contracts.DataContracts
{
    public record DerivedDataContract : BaseDataContract
    {
        public DerivedDataContract(string attributeOnBase, string attributeOnDerived) : base(attributeOnBase)
        {
            AttributeOnDerived = attributeOnDerived;
        }

        /// <summary>
        /// Required by Entity Framework.
        /// </summary>
        [IntentManaged(Mode.Fully)]
        protected DerivedDataContract()
        {
            AttributeOnDerived = null!;
        }

        public string AttributeOnDerived { get; init; }
    }
}