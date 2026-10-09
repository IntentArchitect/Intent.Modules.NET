using CqrsSpike.Publisher.Domain.Common.Exceptions;
using CqrsSpike.Publisher.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using Mediator;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Mediator.CommandHandler", Version = "2.0")]

namespace CqrsSpike.Publisher.Application.Orders.MarkOrderShipped
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class MarkOrderShippedCommandHandler : ICommandHandler<MarkOrderShippedCommand>
    {
        private readonly IOrderRepository _orderRepository;

        [IntentManaged(Mode.Merge)]
        public MarkOrderShippedCommandHandler(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async ValueTask<Unit> Handle(MarkOrderShippedCommand request, CancellationToken cancellationToken)
        {
            var order = await _orderRepository.FindByIdAsync(request.OrderId, cancellationToken);
            if (order is null)
            {
                throw new NotFoundException($"Could not find Order '{request.OrderId}'");
            }

            order.MarkShipped(request.ShipmentId);
            return Unit.Value;
        }
    }
}