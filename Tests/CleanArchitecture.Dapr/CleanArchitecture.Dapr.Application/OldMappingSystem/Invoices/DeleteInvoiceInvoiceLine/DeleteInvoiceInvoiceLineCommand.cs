using CleanArchitecture.Dapr.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace CleanArchitecture.Dapr.Application.OldMappingSystem.Invoices.DeleteInvoiceInvoiceLine
{
    public class DeleteInvoiceInvoiceLineCommand : IRequest, ICommand
    {
        public DeleteInvoiceInvoiceLineCommand(string invoiceId, string id)
        {
            InvoiceId = invoiceId;
            Id = id;
        }

        public string InvoiceId { get; set; }
        public string Id { get; set; }
    }
}