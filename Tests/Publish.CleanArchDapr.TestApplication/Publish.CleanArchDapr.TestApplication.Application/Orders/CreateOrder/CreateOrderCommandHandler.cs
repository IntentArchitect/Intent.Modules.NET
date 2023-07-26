using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using Publish.CleanArchDapr.TestApplication.Application.Common.Eventing;
using Publish.CleanArchDapr.TestApplication.Domain.Entities;
using Publish.CleanArchDapr.TestApplication.Domain.Repositories;
using Publish.CleanArchDapr.TestApplication.Eventing.Messages;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace Publish.CleanArchDapr.TestApplication.Application.Orders.CreateOrder
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, Guid>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IEventBus _eventBus;

        [IntentManaged(Mode.Ignore)]
        public CreateOrderCommandHandler(IOrderRepository orderRepository, IEventBus eventBus)
        {
            _orderRepository = orderRepository;
            _eventBus = eventBus;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<Guid> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
        {
            var newOrder = new Order
            {
                CustomerId = request.CustomerId,
            };

            _orderRepository.Add(newOrder);
            await _orderRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
            _eventBus.Publish(newOrder.MapToOrderCreatedEvent());
            return newOrder.Id;
        }
    }
}