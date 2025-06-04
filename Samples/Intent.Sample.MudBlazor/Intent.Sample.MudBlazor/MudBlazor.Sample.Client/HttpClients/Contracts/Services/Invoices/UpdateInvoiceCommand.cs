using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.Blazor.HttpClients.DtoContract", Version = "2.0")]

namespace MudBlazor.Sample.Client.HttpClients.Contracts.Services.Invoices
{
    public class UpdateInvoiceCommand
    {
        public UpdateInvoiceCommand()
        {
            InvoiceNo = null!;
            OrderLines = [];
        }

        public Guid Id { get; set; }
        public string InvoiceNo { get; set; }
        public DateTime InvoiceDate { get; set; }
        public DateTime DueDate { get; set; }
        public string? Reference { get; set; }
        public Guid CustomerId { get; set; }
        public List<UpdateInvoiceCommandOrderLinesDto> OrderLines { get; set; }

        public static UpdateInvoiceCommand Create(
            Guid id,
            string invoiceNo,
            DateTime invoiceDate,
            DateTime dueDate,
            string? reference,
            Guid customerId,
            List<UpdateInvoiceCommandOrderLinesDto> orderLines)
        {
            return new UpdateInvoiceCommand
            {
                Id = id,
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