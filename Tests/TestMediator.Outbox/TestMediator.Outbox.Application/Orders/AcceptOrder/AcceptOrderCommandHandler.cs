using Intent.RoslynWeaver.Attributes;
using Mediator;
using TestMediator.Outbox.Application.Common.Eventing;
using TestMediator.Outbox.Domain;
using TestMediator.Outbox.Domain.Repositories;
using TestMediator.Outbox.Eventing.Messages;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Mediator.CommandHandler", Version = "1.0")]

namespace TestMediator.Outbox.Application.Orders.AcceptOrder
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class AcceptOrderCommandHandler : ICommandHandler<AcceptOrderCommand>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IMessageBus _messageBus;

        [IntentManaged(Mode.Merge)]
        public AcceptOrderCommandHandler(IOrderRepository orderRepository, IMessageBus messageBus)
        {
            _orderRepository = orderRepository;
            _messageBus = messageBus;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async ValueTask<Unit> Handle(AcceptOrderCommand request, CancellationToken cancellationToken)
        {
            _orderRepository.Add(new Order { Id = request.OrderId, CustomerName = request.CustomerName });
            _messageBus.Publish(new OrderAcceptedEvent { OrderId = request.OrderId });
            await Task.CompletedTask;
            return Unit.Value;
        }
    }
}
