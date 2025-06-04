using AspNetCoreCleanArchitecture.Sample.Domain.Entities;
using AspNetCoreCleanArchitecture.Sample.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace AspNetCoreCleanArchitecture.Sample.Application.Orders.CreateOrder
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
                OrderNo = request.OrderNo,
                OrderDate = request.OrderDate,
                BuyerId = request.BuyerId,
                OrderLines = request.OrderLines
                    .Select(ol => new OrderLine
                    {
                        ProductId = ol.ProductId,
                        Units = ol.Units,
                        UnitPrice = ol.UnitPrice,
                        Discount = ol.Discount
                    })
                    .ToList()
            };

            _orderRepository.Add(order);
            await _orderRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
            return order.Id;
        }
    }
}