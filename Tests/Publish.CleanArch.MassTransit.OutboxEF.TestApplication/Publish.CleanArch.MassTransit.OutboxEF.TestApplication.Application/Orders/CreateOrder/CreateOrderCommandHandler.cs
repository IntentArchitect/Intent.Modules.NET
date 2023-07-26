using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MassTransit.Messages.Shared;
using MediatR;
using Publish.CleanArch.MassTransit.OutboxEF.TestApplication.Application.Common.Eventing;
using Publish.CleanArch.MassTransit.OutboxEF.TestApplication.Domain.Entities;
using Publish.CleanArch.MassTransit.OutboxEF.TestApplication.Domain.Repositories;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace Publish.CleanArch.MassTransit.OutboxEF.TestApplication.Application.Orders.CreateOrder
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, Guid>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IEventBus _eventBus;

        [IntentManaged(Mode.Merge)]
        public CreateOrderCommandHandler(IOrderRepository orderRepository, IEventBus eventBus)
        {
            _orderRepository = orderRepository;
            _eventBus = eventBus;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<Guid> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
        {
            var newOrder = new Order
            {
                Number = request.Number,
                OrderItems = request.OrderItems.Select(CreateOrderItem).ToList(),
            };

            _orderRepository.Add(newOrder);
            await _orderRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
            _eventBus.Publish(newOrder.MapToOrderCreatedEvent());
            return newOrder.Id;
        }

        [IntentManaged(Mode.Fully)]
        private static OrderItem CreateOrderItem(CreateOrderOrderItemDto dto)
        {
            return new OrderItem
            {
                Description = dto.Description,
                Amount = dto.Amount,
            };
        }
    }
}