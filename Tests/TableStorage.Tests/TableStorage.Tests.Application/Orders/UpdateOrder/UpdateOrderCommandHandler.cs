using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using TableStorage.Tests.Domain.Common;
using TableStorage.Tests.Domain.Common.Exceptions;
using TableStorage.Tests.Domain.Entities;
using TableStorage.Tests.Domain.Repositories;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace TableStorage.Tests.Application.Orders.UpdateOrder
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
            var existingOrder = await _orderRepository.FindByIdAsync((request.PartitionKey, request.RowKey), cancellationToken);
            if (existingOrder is null)
            {
                throw new NotFoundException($"Could not find Order '({request.PartitionKey}, {request.RowKey})'");
            }

            existingOrder.PartitionKey = request.PartitionKey;
            existingOrder.RowKey = request.RowKey;
            existingOrder.OrderNo = request.OrderNo;
            existingOrder.Amount = request.Amount;
            existingOrder.Customer = CreateOrUpdateCustomer(existingOrder.Customer, request.Customer);

            _orderRepository.Update(existingOrder);
        }

        [IntentManaged(Mode.Fully)]
        private static Customer CreateOrUpdateCustomer(Customer? entity, UpdateOrderCustomerDto dto)
        {
            entity ??= new Customer();
            entity.Name = dto.Name;

            return entity;
        }
    }
}