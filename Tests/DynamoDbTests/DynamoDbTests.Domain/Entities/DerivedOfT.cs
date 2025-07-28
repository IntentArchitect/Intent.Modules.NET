using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace DynamoDbTests.Domain.Entities
{
    public class DerivedOfT : BaseOfT<int>
    {
        public DerivedOfT()
        {
            DerivedAttribute = null!;
        }

        public string DerivedAttribute { get; set; }
    }
}