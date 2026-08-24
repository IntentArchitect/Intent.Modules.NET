using Intent.RoslynWeaver.Attributes;
using WolverineEventing.Publish.RabbitMQ.Application.Common.Eventing;
using WolverineEventing.Publish.RabbitMQ.Eventing.Messages;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Wolverine.CommandHandler", Version = "1.0")]

namespace WolverineEventing.Publish.RabbitMQ.Application.Orders.ShipOrder
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class ShipOrderCommandHandler
    {
        private readonly IMessageBus _messageBus;

        [IntentManaged(Mode.Merge)]
        public ShipOrderCommandHandler(IMessageBus messageBus)
        {
            _messageBus = messageBus;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task Handle(ShipOrderCommand request, CancellationToken cancellationToken)
        {
            _messageBus.Publish(new OrderShippedEvent
            {
                OrderId = request.OrderId,
                ShippedAt = DateTime.UtcNow
            });
        }
    }
}