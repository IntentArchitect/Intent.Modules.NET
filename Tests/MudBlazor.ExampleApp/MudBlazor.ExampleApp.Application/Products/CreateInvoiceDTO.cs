using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace MudBlazor.ExampleApp.Application.Products
{
    public class CreateInvoiceDTO
    {
        public CreateInvoiceDTO()
        {
            InvoiceNo = null!;
        }

        public string InvoiceNo { get; set; }
        public DateTime IssuedDate { get; set; }
        public DateTime DueDate { get; set; }
        public string? Reference { get; set; }
        public Guid CustomerId { get; set; }

        public static CreateInvoiceDTO Create(
            string invoiceNo,
            DateTime issuedDate,
            DateTime dueDate,
            string? reference,
            Guid customerId)
        {
            return new CreateInvoiceDTO
            {
                InvoiceNo = invoiceNo,
                IssuedDate = issuedDate,
                DueDate = dueDate,
                Reference = reference,
                CustomerId = customerId
            };
        }
    }
}