using Intent.RoslynWeaver.Attributes;
using MediatR;
using ObjectMapping.Strict.Domain.Repositories;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryHandler", Version = "1.0")]

namespace ObjectMapping.Strict.Application.Orders.GetOrderOrNull
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetOrderOrNullHandler : IRequestHandler<GetOrderOrNull, OrderDto?>
    {
        private readonly IOrderRepository _orderRepository;

        [IntentManaged(Mode.Merge)]
        public GetOrderOrNullHandler(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        /// <summary>
        /// Returns a mapped OrderDto or null when no order matches. Pins the null-conditional Call Site shape (R3.4).
        /// </summary>
        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public async Task<OrderDto?> Handle(GetOrderOrNull request, CancellationToken cancellationToken)
        {
            var order = await _orderRepository.FindByIdAsync(request.Id, cancellationToken);
            return order?.MapToOrderDto();
        }
    }
}
