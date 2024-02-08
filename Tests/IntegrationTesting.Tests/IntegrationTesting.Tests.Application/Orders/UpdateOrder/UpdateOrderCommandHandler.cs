using System;
using System.Threading;
using System.Threading.Tasks;
using IntegrationTesting.Tests.Domain.Common;
using IntegrationTesting.Tests.Domain.Common.Exceptions;
using IntegrationTesting.Tests.Domain.Entities;
using IntegrationTesting.Tests.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace IntegrationTesting.Tests.Application.Orders.UpdateOrder
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
            var order = await _orderRepository.FindByIdAsync(request.Id, cancellationToken);
            if (order is null)
            {
                throw new NotFoundException($"Could not find Order '{request.Id}'");
            }

            order.CustomerId = request.CustomerId;
            order.RefNo = request.RefNo;
            order.OrderItems = UpdateHelper.CreateOrUpdateCollection(order.OrderItems, request.OrderItems, (e, d) => e.Id == d.Id, CreateOrUpdateOrderItem);
        }

        [IntentManaged(Mode.Fully)]
        private static OrderItem CreateOrUpdateOrderItem(OrderItem? entity, UpdateOrderCommandOrderItemsDto dto)
        {
            entity ??= new OrderItem();
            entity.Id = dto.Id;
            entity.Description = dto.Description;
            entity.ProductId = dto.ProductId;
            return entity;
        }
    }
}