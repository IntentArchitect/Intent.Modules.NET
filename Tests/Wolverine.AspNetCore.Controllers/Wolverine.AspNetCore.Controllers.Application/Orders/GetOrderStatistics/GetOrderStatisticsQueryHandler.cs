using Intent.RoslynWeaver.Attributes;
using Wolverine.AspNetCore.Controllers.Domain;
using Wolverine.AspNetCore.Controllers.Domain.Repositories;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Wolverine.QueryHandler", Version = "1.0")]

namespace Wolverine.AspNetCore.Controllers.Application.GetOrderStatistics
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetOrderStatisticsQueryHandler
    {
        private readonly IOrderRepository _orderRepository;

        [IntentManaged(Mode.Merge)]
        public GetOrderStatisticsQueryHandler(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
        public async Task<OrderStatisticsDto> Handle(GetOrderStatisticsQuery request, CancellationToken cancellationToken)
        {
            var orders = await _orderRepository.FindAllAsync(cancellationToken);
            return new OrderStatisticsDto
            {
                TotalOrders = orders.Count,
                PendingOrders = orders.Count(o => o.Status == OrderStatus.Pending),
                TotalRevenue = orders.SelectMany(o => o.OrderItems).Sum(i => i.UnitPrice * i.Quantity)
            };
        }
    }
}
