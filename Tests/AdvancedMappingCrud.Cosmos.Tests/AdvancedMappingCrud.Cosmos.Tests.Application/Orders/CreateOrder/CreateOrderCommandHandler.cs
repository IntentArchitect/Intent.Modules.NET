using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AdvancedMappingCrud.Cosmos.Tests.Domain;
using AdvancedMappingCrud.Cosmos.Tests.Domain.Entities;
using AdvancedMappingCrud.Cosmos.Tests.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace AdvancedMappingCrud.Cosmos.Tests.Application.Orders.CreateOrder
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, string>
    {
        private readonly IOrderRepository _orderRepository;

        [IntentManaged(Mode.Merge)]
        public CreateOrderCommandHandler(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<string> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
        {
            var order = new Order
            {
                CustomerId = request.CustomerId,
                RefNo = request.RefNo,
                OrderDate = request.OrderDate,
                OrderStatus = request.OrderStatus,
                OrderItems = request.OrderItems
                    .Select(oi => new OrderItem
                    {
                        Quantity = oi.Quantity,
                        Amount = oi.Amount,
                        ProductId = oi.ProductId
                    })
                    .ToList(),
                OrderTags = request.OrderTags
                    .Select(ot => new OrderTags(
                        name: ot.Name,
                        value: ot.Value))
                    .ToList()
            };

            _orderRepository.Add(order);
            await _orderRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
            return order.Id;
        }
    }
}