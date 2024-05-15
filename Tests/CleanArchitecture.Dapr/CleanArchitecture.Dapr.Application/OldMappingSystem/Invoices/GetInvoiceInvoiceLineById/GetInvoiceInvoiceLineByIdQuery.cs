using CleanArchitecture.Dapr.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryModels", Version = "1.0")]

namespace CleanArchitecture.Dapr.Application.OldMappingSystem.Invoices.GetInvoiceInvoiceLineById
{
    public class GetInvoiceInvoiceLineByIdQuery : IRequest<InvoiceInvoiceLineDto>, IQuery
    {
        public GetInvoiceInvoiceLineByIdQuery(string invoiceId, string id)
        {
            InvoiceId = invoiceId;
            Id = id;
        }

        public string InvoiceId { get; set; }
        public string Id { get; set; }
    }
}