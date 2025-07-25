using Intent.RoslynWeaver.Attributes;
using MediatR;
using TableStorage.Tests.Application.Common.Interfaces;
using TableStorage.Tests.Application.Common.Pagination;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryModels", Version = "1.0")]

namespace TableStorage.Tests.Application.Invoices.GetPagedInvoices
{
    public class GetPagedInvoicesQuery : IRequest<CursorPagedResult<InvoiceDto>>, IQuery
    {
        public GetPagedInvoicesQuery(string partitionKey, int pageSize, string? cursorToken)
        {
            PartitionKey = partitionKey;
            PageSize = pageSize;
            CursorToken = cursorToken;
        }

        public string PartitionKey { get; set; }
        public int PageSize { get; set; }
        public string? CursorToken { get; set; }
    }
}