using CosmosDB.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryModels", Version = "1.0")]

namespace CosmosDB.Application.Invoices.GetInvoiceLineItemById
{
    public class GetInvoiceLineItemByIdQuery : IRequest<InvoiceLineItemDto>, IQuery
    {
        public GetInvoiceLineItemByIdQuery(string invoiceId, string id)
        {
            InvoiceId = invoiceId;
            Id = id;
        }

        public string InvoiceId { get; set; }
        public string Id { get; set; }
    }
}