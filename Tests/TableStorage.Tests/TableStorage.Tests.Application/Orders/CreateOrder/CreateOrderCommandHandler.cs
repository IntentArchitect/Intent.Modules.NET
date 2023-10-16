using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using TableStorage.Tests.Domain.Entities;
using TableStorage.Tests.Domain.Repositories;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace TableStorage.Tests.Application.Orders.CreateOrder
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand>
    {
        private readonly IOrderRepository _orderRepository;

        [IntentManaged(Mode.Merge)]
        public CreateOrderCommandHandler(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task Handle(CreateOrderCommand request, CancellationToken cancellationToken)
        {
            var newOrder = new Order
            {
                PartitionKey = request.PartitionKey,
                RowKey = request.RowKey,
                OrderNo = request.OrderNo,
                Amount = request.Amount,
                Customer = CreateCustomer(request.Customer),
            };

            _orderRepository.Add(newOrder);
        }

        [IntentManaged(Mode.Fully)]
        private static Customer CreateCustomer(CreateOrderCustomerDto dto)
        {
            return new Customer
            {
                Name = dto.Name,
            };
        }
    }
}