using CqrsSpike.Publisher.Application.Common.Eventing;
using CqrsSpike.Publisher.Domain.Entities;
using CqrsSpike.Publisher.Domain.Repositories;
using CqrsSpike.Publisher.Eventing.Messages;
using CqrsSpike.Subscriber.Eventing.Messages;
using Intent.RoslynWeaver.Attributes;
using Mediator;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Mediator.CommandHandler", Version = "2.0")]

namespace CqrsSpike.Publisher.Application.Orders.CreateOrder
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

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async ValueTask<Guid> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
        {
            var order = new Order(
                customerName: request.CustomerName,
                total: request.Total);

            _orderRepository.Add(order);
            await _orderRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
            _messageBus.Publish(new OrderCreatedEvent
            {
                OrderId = order.Id,
                CustomerName = request.CustomerName,
                Total = request.Total
            });
            _messageBus.Send(new RequestShipmentCommand
            {
                OrderId = order.Id
            });
            return order.Id;
        }
    }
}