using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CosmosDB.Domain.Common.Exceptions;
using CosmosDB.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace CosmosDB.Application.Orders.UpdateOrder
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
            var order = await _orderRepository.FindByIdAsync((request.Id, request.WarehouseId), cancellationToken);
            if (order is null)
            {
                throw new NotFoundException($"Could not find Order '({request.Id}, {request.WarehouseId})'");
            }

            order.WarehouseId = request.WarehouseId;
            order.RefNo = request.RefNo;
            order.OrderDate = request.OrderDate;

            _orderRepository.Update(order);
        }
    }
}