using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.Blazor.HttpClients.DtoContract", Version = "2.0")]

namespace MudBlazor.Sample.Client.HttpClients.Contracts.Services.Invoices
{
    public class CreateInvoiceCommand
    {
        public CreateInvoiceCommand()
        {
            InvoiceNo = null!;
            OrderLines = [];
        }

        public string InvoiceNo { get; set; }
        public DateTime InvoiceDate { get; set; }
        public DateTime DueDate { get; set; }
        public string? Reference { get; set; }
        public Guid CustomerId { get; set; }
        public List<CreateInvoiceCommandOrderLinesDto> OrderLines { get; set; }

        public static CreateInvoiceCommand Create(
            string invoiceNo,
            DateTime invoiceDate,
            DateTime dueDate,
            string? reference,
            Guid customerId,
            List<CreateInvoiceCommandOrderLinesDto> orderLines)
        {
            return new CreateInvoiceCommand
            {
                InvoiceNo = invoiceNo,
                InvoiceDate = invoiceDate,
                DueDate = dueDate,
                Reference = reference,
                CustomerId = customerId,
                OrderLines = orderLines
            };
        }
    }
}