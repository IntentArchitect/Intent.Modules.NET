using Intent.RoslynWeaver.Attributes;
using MediatR;
using ObjectMapping.Lenient.Application.Common.Pagination;
using ObjectMapping.Lenient.Domain.Repositories;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryHandler", Version = "1.0")]

namespace ObjectMapping.Lenient.Application.Orders.GetOrdersPaged
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetOrdersPagedHandler : IRequestHandler<GetOrdersPaged, PagedResult<OrderDto>>
    {
        private readonly IOrderRepository _orderRepository;

        [IntentManaged(Mode.Merge)]
        public GetOrdersPagedHandler(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        /// <summary>
        /// Returns a page of mapped OrderDtos. Pins the paged Call Site shape and page metadata pass-through (R4.1, R4.2).
        /// </summary>
        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public async Task<PagedResult<OrderDto>> Handle(GetOrdersPaged request, CancellationToken cancellationToken)
        {
            var orders = await _orderRepository.FindAllAsync(request.PageNo, request.PageSize, cancellationToken);
            return orders.MapToPagedResult(x => x.MapToOrderDto());
        }
    }
}
