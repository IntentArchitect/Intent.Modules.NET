using CqrsSpike.Publisher.Domain.Common.Exceptions;
using CqrsSpike.Publisher.Domain.Repositories;
using Mediator;

namespace CqrsSpike.Publisher.Application.Orders.DeleteOrder
{
    public class DeleteOrderCommandHandler : ICommandHandler<DeleteOrderCommand>
    {
        private readonly IOrderRepository _orderRepository;

        public DeleteOrderCommandHandler(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        public async ValueTask<Unit> Handle(DeleteOrderCommand request, CancellationToken cancellationToken)
        {
            var order = await _orderRepository.FindByIdAsync(request.Id, cancellationToken);
            if (order is null)
            {
                throw new NotFoundException($"Could not find Order '{request.Id}'");
            }


            _orderRepository.Remove(order);
            return Unit.Value;
        }
    }
}