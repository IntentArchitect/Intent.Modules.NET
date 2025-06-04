using Intent.RoslynWeaver.Attributes;
using MediatR;
using MudBlazor.Sample.Application.Common.Interfaces;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace MudBlazor.Sample.Application.Invoices.CreateInvoice
{
    public class CreateInvoiceCommand : IRequest<Guid>, ICommand
    {
        public CreateInvoiceCommand(string invoiceNo,
            DateTime invoiceDate,
            DateTime dueDate,
            string? reference,
            Guid customerId,
            List<CreateInvoiceCommandOrderLinesDto> orderLines)
        {
            InvoiceNo = invoiceNo;
            InvoiceDate = invoiceDate;
            DueDate = dueDate;
            Reference = reference;
            CustomerId = customerId;
            OrderLines = orderLines;
        }

        public string InvoiceNo { get; set; }
        public DateTime InvoiceDate { get; set; }
        public DateTime DueDate { get; set; }
        public string? Reference { get; set; }
        public Guid CustomerId { get; set; }
        public List<CreateInvoiceCommandOrderLinesDto> OrderLines { get; set; }
    }
}