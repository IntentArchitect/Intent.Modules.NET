using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AdvancedMappingCrud.RichDomain.Tests.Domain.Entities;
using AdvancedMappingCrud.RichDomain.Tests.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace AdvancedMappingCrud.RichDomain.Tests.Application.Orders.CreateOrderStatic
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class CreateOrderStaticCommandHandler : IRequestHandler<CreateOrderStaticCommand, Guid>
    {
        private readonly IOrderRepository _orderRepository;

        [IntentManaged(Mode.Merge)]
        public CreateOrderStaticCommandHandler(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<Guid> Handle(CreateOrderStaticCommand request, CancellationToken cancellationToken)
        {
            var order = Order.StaticConstructor(request.RefNo, request.OrderDate, request.OrderItems
                .Select(i => OrderItem.OrderLineStaticConstructor(i.ProductId, i.Quantity, i.Amount))
                .ToList());

            _orderRepository.Add(order);
            await _orderRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
            return order.Id;
        }
    }
}