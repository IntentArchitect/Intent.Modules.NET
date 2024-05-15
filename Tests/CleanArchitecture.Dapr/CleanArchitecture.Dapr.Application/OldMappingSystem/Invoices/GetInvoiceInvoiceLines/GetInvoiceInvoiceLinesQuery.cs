using System.Collections.Generic;
using CleanArchitecture.Dapr.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryModels", Version = "1.0")]

namespace CleanArchitecture.Dapr.Application.OldMappingSystem.Invoices.GetInvoiceInvoiceLines
{
    public class GetInvoiceInvoiceLinesQuery : IRequest<List<InvoiceInvoiceLineDto>>, IQuery
    {
        public GetInvoiceInvoiceLinesQuery(string invoiceId)
        {
            InvoiceId = invoiceId;
        }

        public string InvoiceId { get; set; }
    }
}