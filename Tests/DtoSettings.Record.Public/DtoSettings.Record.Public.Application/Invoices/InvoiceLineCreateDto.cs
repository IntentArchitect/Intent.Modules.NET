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

        public string Description { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; }

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