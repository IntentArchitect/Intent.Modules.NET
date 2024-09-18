using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CosmosDB.Domain.Common.Exceptions;
using CosmosDB.Domain.Entities;
using CosmosDB.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace CosmosDB.Application.Orders.CreateOrderOrderItem
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class CreateOrderOrderItemCommandHandler : IRequestHandler<CreateOrderOrderItemCommand, string>
    {
        private readonly IOrderRepository _orderRepository;

        [IntentManaged(Mode.Merge)]
        public CreateOrderOrderItemCommandHandler(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<string> Handle(CreateOrderOrderItemCommand request, CancellationToken cancellationToken)
        {
            var order = await _orderRepository.FindByIdAsync((request.OrderId, request.WarehouseId), cancellationToken);
            if (order is null)
            {
                throw new NotFoundException($"Could not find OrderItem '{(request.OrderId, request.WarehouseId)}'");
            }
            var orderItem = new OrderItem
            {
                Quantity = request.Quantity,
                Description = request.Description,
                Amount = request.Amount
            };

            order.OrderItems.Add(orderItem);
            _orderRepository.Update(order);
            await _orderRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
            return orderItem.Id;
        }
    }
}