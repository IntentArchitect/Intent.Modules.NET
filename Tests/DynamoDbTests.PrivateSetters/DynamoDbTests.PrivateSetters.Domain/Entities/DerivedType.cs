using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace DynamoDbTests.PrivateSetters.Domain.Entities
{
    public class DerivedType : BaseType
    {
        public DerivedType()
        {
            DerivedTypeAggregateId = null!;
        }

        public string DerivedTypeAggregateId { get; private set; }
    }
}