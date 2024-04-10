using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace DtoSettings.Class.Init.Application.InvoicesAdvanced
{
    public class CreateInvoiceCommandInvoiceLinesDto
    {
        public CreateInvoiceCommandInvoiceLinesDto()
        {
            Description = null!;
            Currency = null!;
        }

        public string Description { get; init; }
        public decimal Amount { get; init; }
        public string Currency { get; init; }

        public static CreateInvoiceCommandInvoiceLinesDto Create(string description, decimal amount, string currency)
        {
            return new CreateInvoiceCommandInvoiceLinesDto
            {
                Description = description,
                Amount = amount,
                Currency = currency
            };
        }
    }
}