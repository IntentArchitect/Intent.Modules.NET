using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace DtoSettings.Record.Public.Application.Invoices
{
    public record InvoiceLineCreateDto
    {
        public InvoiceLineCreateDto()
        {
            Description = null!;
            Currency = null!;
        }

        public string Description { get; init; }
        public decimal Amount { get; init; }
        public string Currency { get; init; }

        public static InvoiceLineCreateDto Create(string description, decimal amount, string currency)
        {
            return new InvoiceLineCreateDto
            {
                Description = description,
                Amount = amount,
                Currency = currency
            };
        }
    }
}