using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace DtoSettings.Class.Internal.Application.InvoicesAdvanced
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

        public string Description { get; internal set; }
        public decimal Amount { get; internal set; }
        public string Currency { get; internal set; }
    }
}