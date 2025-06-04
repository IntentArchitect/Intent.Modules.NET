using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.Blazor.HttpClients.DtoContract", Version = "2.0")]

namespace MudBlazor.Sample.Client.HttpClients.Contracts.Services.Invoices
{
    public class InvoiceDto
    {
        public InvoiceDto()
        {
            InvoiceNo = null!;
            OrderLines = [];
            CustomerName = null!;
            AddressLine1 = null!;
            AddressCity = null!;
            AddressCountry = null!;
            AddressPostal = null!;
        }

        public Guid Id { get; set; }
        public string InvoiceNo { get; set; }
        public DateTime? InvoiceDate { get; set; }
        public List<InvoiceInvoiceLineDto> OrderLines { get; set; }
        public Guid? CustomerId { get; set; }
        public string CustomerName { get; set; }
        public string? CustomerAccountNo { get; set; }
        public string AddressLine1 { get; set; }
        public string? AddressLine2 { get; set; }
        public string AddressCity { get; set; }
        public string AddressCountry { get; set; }
        public string AddressPostal { get; set; }
        public DateTime? DueDate { get; set; }
        public string? Reference { get; set; }

        public static InvoiceDto Create(
            Guid id,
            string invoiceNo,
            DateTime? invoiceDate,
            List<InvoiceInvoiceLineDto> orderLines,
            Guid? customerId,
            string customerName,
            string? customerAccountNo,
            string addressLine1,
            string? addressLine2,
            string addressCity,
            string addressCountry,
            string addressPostal,
            DateTime? dueDate,
            string? reference)
        {
            return new InvoiceDto
            {
                Id = id,
                InvoiceNo = invoiceNo,
                InvoiceDate = invoiceDate,
                OrderLines = orderLines,
                CustomerId = customerId,
                CustomerName = customerName,
                CustomerAccountNo = customerAccountNo,
                AddressLine1 = addressLine1,
                AddressLine2 = addressLine2,
                AddressCity = addressCity,
                AddressCountry = addressCountry,
                AddressPostal = addressPostal,
                DueDate = dueDate,
                Reference = reference
            };
        }
    }
}