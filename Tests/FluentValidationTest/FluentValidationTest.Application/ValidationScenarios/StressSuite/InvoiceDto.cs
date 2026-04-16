using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace FluentValidationTest.Application.ValidationScenarios.StressSuite
{
    public record InvoiceDto
    {
        public InvoiceDto()
        {
            InvoiceNumber = null!;
        }

        public string InvoiceNumber { get; init; }
        public decimal Amount { get; init; }
    }
}