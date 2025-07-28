using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace DynamoDbTests.EnumAsStrings.Domain.Entities
{
    public class InvoiceLogo
    {
        public InvoiceLogo()
        {
            Url = null!;
        }

        public string Url { get; set; }
    }
}