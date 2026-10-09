using CqrsSpike.Publisher.Domain.Common.Exceptions;
using CqrsSpike.Publisher.Domain.Repositories;
using Mediator;

namespace CqrsSpike.Publisher.Application.Orders.MarkOrderShipped
{
    public class MarkOrderShippedCommandHandler : ICommandHandler<MarkOrderShippedCommand>
    {
        private readonly IOrderRepository _orderRepository;

        public MarkOrderShippedCommandHandler(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        public async ValueTask<Unit> Handle(MarkOrderShippedCommand request, CancellationToken cancellationToken)
        {
            var order = await _orderRepository.FindByIdAsync(request.OrderId, cancellationToken);
            if (order is null)
            {
                throw new NotFoundException($"Could not find Order '{request.OrderId}'");
            }

            order.MarkShipped(request.ShipmentId);
            return Unit.Value;
        }
    }
}