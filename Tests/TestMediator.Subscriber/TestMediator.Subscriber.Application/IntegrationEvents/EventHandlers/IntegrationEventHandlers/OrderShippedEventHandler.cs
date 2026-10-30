using Intent.RoslynWeaver.Attributes;
using Mediator;
using TestMediator.Publisher.Eventing.Messages;
using TestMediator.Subscriber.Application.Common.Eventing;
using TestMediator.Subscriber.Application.Shipments.CompleteShipment;
using TestMediator.Subscriber.Eventing.Messages;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.Contracts.IntegrationEventHandler", Version = "1.0")]

namespace TestMediator.Subscriber.Application.IntegrationEvents.EventHandlers.IntegrationEventHandlers
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class OrderShippedEventHandler : IIntegrationEventHandler<OrderShippedEvent>
    {
        private readonly ISender _sender;
        private readonly IMessageBus _messageBus;

        [IntentManaged(Mode.Merge)]
        public OrderShippedEventHandler(ISender sender, IMessageBus messageBus)
        {
            _sender = sender;
            _messageBus = messageBus;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task HandleAsync(OrderShippedEvent message, CancellationToken cancellationToken = default)
        {
            await _sender.Send(new CompleteShipmentCommand(message.OrderId), cancellationToken);
            _messageBus.Publish(new ShipmentDispatchedEvent { OrderId = message.OrderId });
        }
    }
}
