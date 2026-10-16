using Intent.RoslynWeaver.Attributes;
using Mediator;
using TestMediator.Publisher.Domain.Common.Exceptions;
using TestMediator.Publisher.Domain.Repositories;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Mediator.QueryHandler", Version = "1.0")]

namespace TestMediator.Publisher.Application.Orders.GetOrderById
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetOrderByIdQueryHandler : IQueryHandler<GetOrderByIdQuery, OrderDto>
    {
        private readonly IOrderRepository _orderRepository;

        [IntentManaged(Mode.Merge)]
        public GetOrderByIdQueryHandler(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async ValueTask<OrderDto> Handle(GetOrderByIdQuery request, CancellationToken cancellationToken)
        {
            var order = await _orderRepository.FindByIdAsync(request.Id, cancellationToken);
            if (order is null)
            {
                throw new NotFoundException($"Could not find Order '{request.Id}'");
            }

            return new OrderDto
            {
                Id = order.Id,
                CustomerName = order.CustomerName,
                Total = order.Total,
                Status = order.Status
            };
        }
    }
}
