using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace DynamoDbTests.EntityInterfaces.Domain.Entities
{
    public class DerivedOfT : BaseOfT<int>, IDerivedOfT
    {
        public DerivedOfT()
        {
            DerivedAttribute = null!;
        }

        public string DerivedAttribute { get; set; }
    }
}