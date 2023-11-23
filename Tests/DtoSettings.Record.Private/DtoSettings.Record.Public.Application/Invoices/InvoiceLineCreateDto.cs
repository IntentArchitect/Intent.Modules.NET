using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace DtoSettings.Record.Public.Application.Invoices
{
    public record InvoiceLineCreateDto
    {
        public InvoiceLineCreateDto(string description, decimal amount, string currency)
        {
            Description = description;
            Amount = amount;
            Currency = currency;
        }

        protected InvoiceLineCreateDto()
        {
            Description = null!;
            Currency = null!;
        }

        public string Description { get; private set; }
        public decimal Amount { get; private set; }
        public string Currency { get; private set; }

        public static InvoiceLineCreateDto Create(string description, decimal amount, string currency)
        {
            return new InvoiceLineCreateDto(description, amount, currency);
        }
    }
}