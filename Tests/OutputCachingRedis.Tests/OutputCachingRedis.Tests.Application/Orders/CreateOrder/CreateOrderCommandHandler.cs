using System;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using OutputCachingRedis.Tests.Domain.Entities;
using OutputCachingRedis.Tests.Domain.Repositories;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace OutputCachingRedis.Tests.Application.Orders.CreateOrder
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
                RefNo = request.RefNo
            };

            _orderRepository.Add(order);
            await _orderRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
            return order.Id;
        }
    }
}