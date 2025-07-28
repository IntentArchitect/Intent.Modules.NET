using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace DynamoDbTests.EntityInterfaces.Domain.Entities
{
    public class InvoiceLogo : IInvoiceLogo
    {
        public InvoiceLogo()
        {
            Url = null!;
        }

        public string Url { get; set; }
    }
}