using System.Collections.Generic;
using EntityFrameworkCore.Oracle.TestApplication.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryModels", Version = "1.0")]

namespace EntityFrameworkCore.Oracle.TestApplication.Application.Invoices.GetInvoices
{
    public class GetInvoicesQuery : IRequest<List<InvoiceDto>>, IQuery
    {
        public GetInvoicesQuery()
        {
        }
    }
}