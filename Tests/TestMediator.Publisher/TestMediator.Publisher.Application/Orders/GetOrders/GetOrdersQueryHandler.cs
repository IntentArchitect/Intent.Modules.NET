using Intent.RoslynWeaver.Attributes;
using Mediator;
using TestMediator.Publisher.Domain.Repositories;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Mediator.QueryHandler", Version = "1.0")]

namespace TestMediator.Publisher.Application.Orders.GetOrders
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetOrdersQueryHandler : IQueryHandler<GetOrdersQuery, List<OrderDto>>
    {
        private readonly IOrderRepository _orderRepository;

        [IntentManaged(Mode.Merge)]
        public GetOrdersQueryHandler(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async ValueTask<List<OrderDto>> Handle(GetOrdersQuery request, CancellationToken cancellationToken)
        {
            var orders = await _orderRepository.FindAllAsync(cancellationToken);
            return orders.Select(order => new OrderDto
            {
                Id = order.Id,
                CustomerName = order.CustomerName,
                Total = order.Total,
                Status = order.Status
            }).ToList();
        }
    }
}
