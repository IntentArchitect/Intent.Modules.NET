using System.Collections.Generic;
using CosmosDBMultiTenancy.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryModels", Version = "1.0")]

namespace CosmosDBMultiTenancy.Application.Invoices.GetInvoices
{
    public class GetInvoicesQuery : IRequest<List<InvoiceDto>>, IQuery
    {
        public GetInvoicesQuery()
        {
        }
    }
}