using Intent.RoslynWeaver.Attributes;
using Mediator;
using TestMediator.Publisher.Application.Common.Eventing;
using TestMediator.Publisher.Domain;
using TestMediator.Publisher.Domain.Repositories;
using TestMediator.Publisher.Eventing.Messages;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Mediator.CommandHandler", Version = "1.0")]

namespace TestMediator.Publisher.Application.Orders.CreateOrder
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class CreateOrderCommandHandler : ICommandHandler<CreateOrderCommand, Guid>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IMessageBus _messageBus;

        [IntentManaged(Mode.Merge)]
        public CreateOrderCommandHandler(IOrderRepository orderRepository, IMessageBus messageBus)
        {
            _orderRepository = orderRepository;
            _messageBus = messageBus;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async ValueTask<Guid> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
        {
            var order = new Order(request.CustomerName, request.Total)
            {
                Status = OrderStatus.Placed
            };

            _orderRepository.Add(order);
            await _orderRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
            _messageBus.Publish(new OrderCreatedEvent
            {
                OrderId = order.Id,
                CustomerName = request.CustomerName
            });
            return order.Id;
        }
    }
}
