using System.Collections.Generic;
using CosmosDB.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryModels", Version = "1.0")]

namespace CosmosDB.Application.Invoices.GetInvoiceLineItems
{
    public class GetInvoiceLineItemsQuery : IRequest<List<InvoiceLineItemDto>>, IQuery
    {
        public GetInvoiceLineItemsQuery(string invoiceId)
        {
            InvoiceId = invoiceId;
        }

        public string InvoiceId { get; set; }
    }
}