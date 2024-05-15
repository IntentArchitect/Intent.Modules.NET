using CleanArchitecture.Dapr.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace CleanArchitecture.Dapr.Application.OldMappingSystem.Invoices.UpdateInvoiceInvoiceLine
{
    public class UpdateInvoiceInvoiceLineCommand : IRequest, ICommand
    {
        public UpdateInvoiceInvoiceLineCommand(string invoiceId, string id, string description, int quantity)
        {
            InvoiceId = invoiceId;
            Id = id;
            Description = description;
            Quantity = quantity;
        }

        public string InvoiceId { get; set; }
        public string Id { get; set; }
        public string Description { get; set; }
        public int Quantity { get; set; }
    }
}