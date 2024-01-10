using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Entities.Interfaces.EF.Domain.Entities;
using Entities.Interfaces.EF.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace Entities.Interfaces.EF.Application.Orders.CreateOrder
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
            var newOrder = new Order
            {
                RefNo = request.RefNo,
                OrderItems = request.OrderItems.Select(CreateOrderItem).ToList(),
            };

            _orderRepository.Add(newOrder);
            await _orderRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
            return newOrder.Id;
        }

        [IntentManaged(Mode.Fully)]
        private static OrderItem CreateOrderItem(CreateOrderOrderItemDto dto)
        {
            return new OrderItem
            {
                Description = dto.Description,
            };
        }
    }
}