using System;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MassTransit.Messages.Shared;
using MediatR;
using Publish.CleanArch.MassTransit.OutboxEF.TestApplication.Application.Common.Eventing;
using Publish.CleanArch.MassTransit.OutboxEF.TestApplication.Domain.Common.Exceptions;
using Publish.CleanArch.MassTransit.OutboxEF.TestApplication.Domain.Repositories;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "1.0")]

namespace Publish.CleanArch.MassTransit.OutboxEF.TestApplication.Application.Orders.DeleteOrder
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class DeleteOrderCommandHandler : IRequestHandler<DeleteOrderCommand>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IEventBus _eventBus;

        [IntentManaged(Mode.Merge)]
        public DeleteOrderCommandHandler(IOrderRepository orderRepository, IEventBus eventBus)
        {
            _orderRepository = orderRepository;
            _eventBus = eventBus;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<Unit> Handle(DeleteOrderCommand request, CancellationToken cancellationToken)
        {
            var existingOrder = await _orderRepository.FindByIdAsync(request.Id, cancellationToken);

            if (existingOrder is null)
            {
                throw new NotFoundException($"Could not find Order '{request.Id}' ");
            }
            _orderRepository.Remove(existingOrder);
            _eventBus.Publish(existingOrder.MapToOrderDeletedEvent());
            return Unit.Value;
        }
    }
}