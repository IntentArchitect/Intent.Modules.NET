using Intent.RoslynWeaver.Attributes;
using MediatR;
using TableStorage.Tests.Application.Common.Interfaces;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryModels", Version = "1.0")]

namespace TableStorage.Tests.Application.Invoices.GetInvoiceById
{
    public class GetInvoiceByIdQuery : IRequest<InvoiceDto>, IQuery
    {
        public GetInvoiceByIdQuery(string partitionKey, string rowKey)
        {
            PartitionKey = partitionKey;
            RowKey = rowKey;
        }

        public string PartitionKey { get; set; }
        public string RowKey { get; set; }
    }
}