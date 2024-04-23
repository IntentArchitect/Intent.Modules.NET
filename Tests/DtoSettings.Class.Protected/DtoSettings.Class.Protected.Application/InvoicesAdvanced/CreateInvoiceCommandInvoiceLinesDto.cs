using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace DtoSettings.Class.Protected.Application.InvoicesAdvanced
{
    public class CreateInvoiceCommandInvoiceLinesDto
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

        public string Description { get; protected set; }
        public decimal Amount { get; protected set; }
        public string Currency { get; protected set; }

        public static CreateInvoiceCommandInvoiceLinesDto Create(string description, decimal amount, string currency)
        {
            return new CreateInvoiceCommandInvoiceLinesDto(description, amount, currency);
        }
    }
}