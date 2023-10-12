using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace DtoSettings.Class.Internal.Application.Invoices
{
    public class InvoiceLineCreateDto
    {
        public InvoiceLineCreateDto(string description, decimal amount, string currency)
        {
            Description = description;
            Amount = amount;
            Currency = currency;
        }

        public string Description { get; internal set; }
        public decimal Amount { get; internal set; }
        public string Currency { get; internal set; }

        // public static InvoiceLineCreateDto Create(string description, decimal amount, string currency)
        // {
        //     return new InvoiceLineCreateDto
        //     {
        //         Description = description,
        //         Amount = amount,
        //         Currency = currency
        //     };
        // }
    }
}