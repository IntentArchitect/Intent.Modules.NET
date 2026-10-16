using Intent.RoslynWeaver.Attributes;
using TestMediator.Publisher.Eventing.Messages;
using TestMediator.Subscriber.Application.Common.Eventing;
using TestMediator.Subscriber.Eventing.Messages;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.Contracts.IntegrationEventHandler", Version = "1.0")]

namespace TestMediator.Subscriber.Application.IntegrationEvents.EventHandlers.IntegrationEventHandlers
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class OrderRelayHandler : IIntegrationEventHandler<OrderCancelledEvent>
    {
        private readonly IMessageBus _messageBus;

        [IntentManaged(Mode.Merge)]
        public OrderRelayHandler(IMessageBus messageBus)
        {
            _messageBus = messageBus;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task HandleAsync(OrderCancelledEvent message, CancellationToken cancellationToken = default)
        {
            _messageBus.Publish(new OrderRelayedEvent { OrderId = message.OrderId });
            await Task.CompletedTask;
        }
    }
}
