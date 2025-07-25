using Intent.RoslynWeaver.Attributes;
using MediatR;
using TableStorage.Tests.Application.Common.Interfaces;
using TableStorage.Tests.Application.Common.Pagination;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryModels", Version = "1.0")]

namespace TableStorage.Tests.Application.Orders.GetPagedOrders
{
    public class GetPagedOrdersQuery : IRequest<CursorPagedResult<OrderDto>>, IQuery
    {
        public GetPagedOrdersQuery(string partitionKey, int pageSize, string? cursorToken, string? orderNo)
        {
            PartitionKey = partitionKey;
            PageSize = pageSize;
            CursorToken = cursorToken;
            OrderNo = orderNo;
        }

        public string PartitionKey { get; set; }
        public int PageSize { get; set; }
        public string? CursorToken { get; set; }
        public string? OrderNo { get; set; }
    }
}