using Intent.RoslynWeaver.Attributes;
using MediatR;
using Wolverine.Publish.RabbitMQ.Application.Common.Eventing;
using Wolverine.Publish.RabbitMQ.Eventing.Messages;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace Wolverine.Publish.RabbitMQ.Application.ShipOrder
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class ShipOrderCommandHandler : IRequestHandler<ShipOrderCommand>
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
            });
        }
    }
}