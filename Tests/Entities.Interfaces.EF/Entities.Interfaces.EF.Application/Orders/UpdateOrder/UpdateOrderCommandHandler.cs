using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Entities.Interfaces.EF.Domain.Common;
using Entities.Interfaces.EF.Domain.Common.Exceptions;
using Entities.Interfaces.EF.Domain.Entities;
using Entities.Interfaces.EF.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace Entities.Interfaces.EF.Application.Orders.UpdateOrder
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class UpdateOrderCommandHandler : IRequestHandler<UpdateOrderCommand>
    {
        private readonly IOrderRepository _orderRepository;

        [IntentManaged(Mode.Merge)]
        public UpdateOrderCommandHandler(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task Handle(UpdateOrderCommand request, CancellationToken cancellationToken)
        {
            var existingOrder = await _orderRepository.FindByIdAsync(request.Id, cancellationToken);
            if (existingOrder is null)
            {
                throw new NotFoundException($"Could not find Order '{request.Id}'");
            }

            existingOrder.RefNo = request.RefNo;
            existingOrder.OrderItems = UpdateHelper.CreateOrUpdateCollection(existingOrder.OrderItems, request.OrderItems, (e, d) => e.Id == d.Id, CreateOrUpdateOrderItem);
        }

        [IntentManaged(Mode.Fully)]
        private static IOrderItem CreateOrUpdateOrderItem(IOrderItem? entity, UpdateOrderOrderItemDto dto)
        {
            entity ??= new OrderItem();
            entity.Description = dto.Description;
            entity.OrderId = dto.OrderId;

            return entity;
        }
    }
}