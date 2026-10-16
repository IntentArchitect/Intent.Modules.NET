using Intent.RoslynWeaver.Attributes;
using Mediator;
using TestMediator.Outbox.Application.Common.Eventing;
using TestMediator.Outbox.Application.Orders.AcceptOrder;
using TestMediator.Outbox.Eventing.Messages;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.Contracts.IntegrationEventHandler", Version = "1.0")]

namespace TestMediator.Outbox.Application.IntegrationEvents.EventHandlers.IntegrationEventHandlers
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class OrderPlacedEventHandler : IIntegrationEventHandler<OrderPlacedEvent>
    {
        private readonly ISender _sender;

        [IntentManaged(Mode.Merge)]
        public OrderPlacedEventHandler(ISender sender)
        {
            _sender = sender;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task HandleAsync(OrderPlacedEvent message, CancellationToken cancellationToken = default)
        {
            await _sender.Send(new AcceptOrderCommand(message.OrderId, message.CustomerName), cancellationToken);
        }
    }
}
