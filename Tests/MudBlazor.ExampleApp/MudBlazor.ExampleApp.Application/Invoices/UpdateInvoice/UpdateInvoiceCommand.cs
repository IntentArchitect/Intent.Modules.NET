using Intent.RoslynWeaver.Attributes;
using MediatR;
using MudBlazor.ExampleApp.Application.Common.Interfaces;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace MudBlazor.ExampleApp.Application.Invoices.UpdateInvoice
{
    public class UpdateInvoiceCommand : IRequest, ICommand
    {
        public UpdateInvoiceCommand(Guid id,
            string invoiceNo,
            DateTime invoiceDate,
            DateTime dueDate,
            string? reference,
            Guid customerId,
            List<UpdateInvoiceCommandOrderLinesDto> orderLines)
        {
            Id = id;
            InvoiceNo = invoiceNo;
            InvoiceDate = invoiceDate;
            DueDate = dueDate;
            Reference = reference;
            CustomerId = customerId;
            OrderLines = orderLines;
        }

        public Guid Id { get; set; }
        public string InvoiceNo { get; set; }
        public DateTime InvoiceDate { get; set; }
        public DateTime DueDate { get; set; }
        public string? Reference { get; set; }
        public Guid CustomerId { get; set; }
        public List<UpdateInvoiceCommandOrderLinesDto> OrderLines { get; set; }
    }
}