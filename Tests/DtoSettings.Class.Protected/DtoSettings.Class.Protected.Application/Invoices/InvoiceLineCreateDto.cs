using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace DtoSettings.Class.Protected.Application.Invoices
{
    public class InvoiceLineCreateDto
    {
        public InvoiceLineCreateDto()
        {
            Description = null!;
            Currency = null!;
        }

        public string Description { get; protected set; }
        public decimal Amount { get; protected set; }
        public string Currency { get; protected set; }

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