using Intent.RoslynWeaver.Attributes;
using Mediator;
using TestMediator.Publisher.Application.Common.Eventing;
using TestMediator.Publisher.Domain;
using TestMediator.Publisher.Domain.Common.Exceptions;
using TestMediator.Publisher.Domain.Repositories;
using TestMediator.Publisher.Eventing.Messages;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Mediator.CommandHandler", Version = "1.0")]

namespace TestMediator.Publisher.Application.Orders.MarkOrderShipped
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class MarkOrderShippedCommandHandler : ICommandHandler<MarkOrderShippedCommand>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IMessageBus _messageBus;

        [IntentManaged(Mode.Merge)]
        public MarkOrderShippedCommandHandler(IOrderRepository orderRepository, IMessageBus messageBus)
        {
            _orderRepository = orderRepository;
            _messageBus = messageBus;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async ValueTask<Unit> Handle(MarkOrderShippedCommand request, CancellationToken cancellationToken)
        {
            var order = await _orderRepository.FindByIdAsync(request.Id, cancellationToken);
            if (order is null)
            {
                throw new NotFoundException($"Could not find Order '{request.Id}'");
            }

            order.Status = OrderStatus.Shipped;
            _messageBus.Publish(new OrderShippedEvent { OrderId = order.Id });
            return Unit.Value;
        }
    }
}
