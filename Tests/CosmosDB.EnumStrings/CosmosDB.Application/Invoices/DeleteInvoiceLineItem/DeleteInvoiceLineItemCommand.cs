using CosmosDB.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace CosmosDB.Application.Invoices.DeleteInvoiceLineItem
{
    public class DeleteInvoiceLineItemCommand : IRequest, ICommand
    {
        public DeleteInvoiceLineItemCommand(string invoiceId, string id)
        {
            InvoiceId = invoiceId;
            Id = id;
        }

        public string InvoiceId { get; set; }
        public string Id { get; set; }
    }
}