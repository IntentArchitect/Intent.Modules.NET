using CqrsSpike.Publisher.Application.Common.Eventing;
using CqrsSpike.Publisher.Domain.Entities;
using CqrsSpike.Publisher.Domain.Repositories;
using CqrsSpike.Publisher.Eventing.Messages;
using CqrsSpike.Subscriber.Eventing.Messages;
using Mediator;

namespace CqrsSpike.Publisher.Application.Orders.CreateOrder
{
    public class CreateOrderCommandHandler : ICommandHandler<CreateOrderCommand, Guid>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IMessageBus _messageBus;

        public CreateOrderCommandHandler(IOrderRepository orderRepository, IMessageBus messageBus)
        {
            _orderRepository = orderRepository;
            _messageBus = messageBus;
        }

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