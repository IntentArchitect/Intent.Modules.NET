using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;
using Kafka.Producer.Application.Common.Interfaces;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryModels", Version = "1.0")]

namespace Kafka.Producer.Application.Invoices.GetInvoices
{
    public class GetInvoicesQuery : IRequest<List<InvoiceDto>>, IQuery
    {
        public GetInvoicesQuery()
        {
        }
    }
}