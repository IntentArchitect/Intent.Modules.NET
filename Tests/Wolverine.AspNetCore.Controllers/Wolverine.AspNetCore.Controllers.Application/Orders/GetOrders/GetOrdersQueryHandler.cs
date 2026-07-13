using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using Wolverine.AspNetCore.Controllers.Application.Common.Pagination;
using Wolverine.AspNetCore.Controllers.Domain.Repositories;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Wolverine.QueryHandler", Version = "1.0")]

namespace Wolverine.AspNetCore.Controllers.Application.GetOrders
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetOrdersQueryHandler
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IMapper _mapper;
        [IntentManaged(Mode.Merge)]
        public GetOrdersQueryHandler(IOrderRepository orderRepository, IMapper mapper)
        {
            _orderRepository = orderRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Merge, Signature = Mode.Fully, Body = Mode.Fully)]
        public async Task<PagedResult<OrderDto>> Handle(GetOrdersQuery request, CancellationToken cancellationToken)
        {
            var results = await _orderRepository.FindAllAsync(request.PageNo, request.PageSize, cancellationToken);
            return results.MapToPagedResult(x => x.MapToOrderDto(_mapper));
        }
    }
}