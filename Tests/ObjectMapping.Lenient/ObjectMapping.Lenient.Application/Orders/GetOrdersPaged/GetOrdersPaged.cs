using Intent.RoslynWeaver.Attributes;
using MediatR;
using ObjectMapping.Lenient.Application.Common.Interfaces;
using ObjectMapping.Lenient.Application.Common.Pagination;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryModels", Version = "1.0")]

namespace ObjectMapping.Lenient.Application.Orders.GetOrdersPaged
{
    /// <summary>
    /// Returns a page of mapped OrderDtos. Pins the paged Call Site shape and page metadata pass-through (R4.1, R4.2).
    /// </summary>
    public class GetOrdersPaged : IRequest<PagedResult<OrderDto>>, IQuery
    {
        public GetOrdersPaged(int pageNo = 1, int pageSize = 20)
        {
            PageNo = pageNo;
            PageSize = pageSize;
        }

        public int PageNo { get; set; }
        public int PageSize { get; set; }
    }
}