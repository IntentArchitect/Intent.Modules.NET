using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AdvancedMappingCrud.Repositories.Tests.Domain;
using AdvancedMappingCrud.Repositories.Tests.Domain.Entities;
using AdvancedMappingCrud.Repositories.Tests.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.Orders.CreateOrder
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, Guid>
    {
        private readonly IOrderRepository _orderRepository;

        [IntentManaged(Mode.Merge)]
        public CreateOrderCommandHandler(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<Guid> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
        {
            var order = new Order
            {
                RefNo = request.RefNo,
                OrderDate = request.OrderDate,
                OrderStatus = request.OrderStatus,
                CustomerId = request.CustomerId,
                OrderItems = request.OrderItems
                    .Select(oi => new OrderItem
                    {
                        Quantity = oi.Quantity,
                        UnitPrice = oi.Amount,
                        ProductId = oi.ProductId
                    })
                    .ToList(),
                DeliveryAddress = new Address(
                    line1: request.Line1,
                    line2: request.Line2,
                    city: request.City,
                    postal: request.Postal),
                BillingAddress = new Address(
                    line1: request.Line1,
                    line2: request.Line2,
                    city: request.City,
                    postal: request.Postal)
            };

            _orderRepository.Add(order);
            await _orderRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
            return order.Id;
        }
    }
}