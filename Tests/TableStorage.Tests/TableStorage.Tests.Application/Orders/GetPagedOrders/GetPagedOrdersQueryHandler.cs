using System;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using TableStorage.Tests.Application.Common.Pagination;
using TableStorage.Tests.Domain.Repositories;
using TableStorage.Tests.Domain.Repositories.TableEntities;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryHandler", Version = "1.0")]

namespace TableStorage.Tests.Application.Orders.GetPagedOrders
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetPagedOrdersQueryHandler : IRequestHandler<GetPagedOrdersQuery, CursorPagedResult<OrderDto>>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Merge)]
        public GetPagedOrdersQueryHandler(IOrderRepository orderRepository, IMapper mapper)
        {
            _orderRepository = orderRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<CursorPagedResult<OrderDto>> Handle(
            GetPagedOrdersQuery request,
            CancellationToken cancellationToken)
        {
            Expression<Func<IOrderTableEntity, bool>> filterOrders = entity => true;
            filterOrders = filterOrders.Combine(entity => entity.PartitionKey == request.PartitionKey);

            if (request.OrderNo != null)
            {
                var requestField = request.OrderNo;
                filterOrders = filterOrders.Combine(entity => entity.OrderNo == requestField);
            }

            var order = await _orderRepository.FindAllAsync(filterOrders, request.PageSize, request.CursorToken, cancellationToken);
            return order.MapToCursorPagedResult(x => x.MapToOrderDto(_mapper));
        }
    }
}