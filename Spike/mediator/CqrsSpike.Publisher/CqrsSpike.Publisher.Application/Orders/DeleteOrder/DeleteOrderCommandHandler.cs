using CqrsSpike.Publisher.Domain.Common.Exceptions;
using CqrsSpike.Publisher.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using Mediator;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Mediator.CommandHandler", Version = "2.0")]

namespace CqrsSpike.Publisher.Application.Orders.DeleteOrder
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class DeleteOrderCommandHandler : ICommandHandler<DeleteOrderCommand>
    {
        private readonly IOrderRepository _orderRepository;

        [IntentManaged(Mode.Merge)]
        public DeleteOrderCommandHandler(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
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