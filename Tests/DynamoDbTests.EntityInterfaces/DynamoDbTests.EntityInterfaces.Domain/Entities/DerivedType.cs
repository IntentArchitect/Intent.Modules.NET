using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace DynamoDbTests.EntityInterfaces.Domain.Entities
{
    public class DerivedType : BaseType, IDerivedType
    {
        public DerivedType()
        {
            DerivedTypeAggregateId = null!;
        }

        public string DerivedTypeAggregateId { get; set; }
    }
}