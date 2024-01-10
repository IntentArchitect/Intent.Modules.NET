using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MassTransit.Messages.Shared;
using MediatR;
using Publish.CleanArch.MassTransit.OutboxEF.TestApplication.Application.Common.Eventing;
using Publish.CleanArch.MassTransit.OutboxEF.TestApplication.Domain.Common;
using Publish.CleanArch.MassTransit.OutboxEF.TestApplication.Domain.Common.Exceptions;
using Publish.CleanArch.MassTransit.OutboxEF.TestApplication.Domain.Entities;
using Publish.CleanArch.MassTransit.OutboxEF.TestApplication.Domain.Repositories;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace Publish.CleanArch.MassTransit.OutboxEF.TestApplication.Application.Orders.UpdateOrder
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class UpdateOrderCommandHandler : IRequestHandler<UpdateOrderCommand>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IEventBus _eventBus;

        [IntentManaged(Mode.Merge)]
        public UpdateOrderCommandHandler(IOrderRepository orderRepository, IEventBus eventBus)
        {
            _orderRepository = orderRepository;
            _eventBus = eventBus;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task Handle(UpdateOrderCommand request, CancellationToken cancellationToken)
        {
            var existingOrder = await _orderRepository.FindByIdAsync(request.Id, cancellationToken);
            if (existingOrder is null)
            {
                throw new NotFoundException($"Could not find Order '{request.Id}'");
            }

            existingOrder.Number = request.Number;
            existingOrder.OrderItems = UpdateHelper.CreateOrUpdateCollection(existingOrder.OrderItems, request.OrderItems, (e, d) => e.Id == d.Id, CreateOrUpdateOrderItem);
            _eventBus.Publish(existingOrder.MapToOrderUpdatedEvent());

        }

        [IntentManaged(Mode.Fully)]
        private static OrderItem CreateOrUpdateOrderItem(OrderItem? entity, UpdateOrderOrderItemDto dto)
        {
            entity ??= new OrderItem();
            entity.OrderId = dto.OrderId;
            entity.Description = dto.Description;
            entity.Amount = dto.Amount;

            return entity;
        }
    }
}