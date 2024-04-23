using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace DtoSettings.Record.Public.Application.InvoicesAdvanced
{
    public record CreateInvoiceCommandInvoiceLinesDto
    {
        public CreateInvoiceCommandInvoiceLinesDto(string description, decimal amount, string currency)
        {
            Description = description;
            Amount = amount;
            Currency = currency;
        }

        protected CreateInvoiceCommandInvoiceLinesDto()
        {
            Description = null!;
            Currency = null!;
        }

        public string Description { get; private set; }
        public decimal Amount { get; private set; }
        public string Currency { get; private set; }

        public static CreateInvoiceCommandInvoiceLinesDto Create(string description, decimal amount, string currency)
        {
            return new CreateInvoiceCommandInvoiceLinesDto(description, amount, currency);
        }
    }
}