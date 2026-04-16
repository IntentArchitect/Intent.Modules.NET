using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace FluentValidationTest.Domain.Entities.ValidationScenarios.StressSuite
{
    public class Invoice
    {
        public Invoice()
        {
            InvoiceNumber = null!;
        }

        public Guid Id { get; set; }

        public string InvoiceNumber { get; set; }

        public decimal Amount { get; set; }
    }
}