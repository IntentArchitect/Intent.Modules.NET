using CosmosDB.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace CosmosDB.Application.Invoices.CreateInvoiceLineItem
{
    public class CreateInvoiceLineItemCommand : IRequest<string>, ICommand
    {
        public CreateInvoiceLineItemCommand(string invoiceId, string description, int quantity)
        {
            InvoiceId = invoiceId;
            Description = description;
            Quantity = quantity;
        }

        public string InvoiceId { get; set; }
        public string Description { get; set; }
        public int Quantity { get; set; }
    }
}