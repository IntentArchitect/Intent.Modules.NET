using Intent.RoslynWeaver.Attributes;
using MediatR;
using ObjectMapping.Lenient.Domain.Repositories;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryHandler", Version = "1.0")]

namespace ObjectMapping.Lenient.Application.Orders.GetOrders
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetOrdersHandler : IRequestHandler<GetOrders, List<OrderDto>>
    {
        private readonly IOrderRepository _orderRepository;

        [IntentManaged(Mode.Merge)]
        public GetOrdersHandler(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        /// <summary>
        /// Returns every order as a collection of mapped DTOs. Pins the List Call Site shape (R3.3).
        /// </summary>
        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public async Task<List<OrderDto>> Handle(GetOrders request, CancellationToken cancellationToken)
        {
            var orders = await _orderRepository.FindAllAsync(cancellationToken);
            return orders.MapToOrderDtoList();
        }
    }
}
