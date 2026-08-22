using Intent.RoslynWeaver.Attributes;
using Wolverine.Publish.RabbitMQ.Application.Common.Eventing;
using Wolverine.Publish.RabbitMQ.Eventing.Messages;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Wolverine.CommandHandler", Version = "1.0")]

namespace Wolverine.Publish.RabbitMQ.Application.ShipOrder
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class ShipOrderCommandHandler
    {
        private readonly global::Wolverine.Publish.RabbitMQ.Application.Common.Eventing.IMessageBus _messageBus;

        [IntentManaged(Mode.Merge)]
        public ShipOrderCommandHandler(global::Wolverine.Publish.RabbitMQ.Application.Common.Eventing.IMessageBus messageBus)
        {
            _messageBus = messageBus;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task Handle(ShipOrderCommand request, CancellationToken cancellationToken)
        {
            _messageBus.Publish(new OrderShippedEvent
            {
            });
        }
    }
}