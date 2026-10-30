using Intent.RoslynWeaver.Attributes;
using Mediator;
using TestMediator.Publisher.Application.Common.Eventing;
using TestMediator.Publisher.Application.Orders.MarkOrderShipped;
using TestMediator.Publisher.Eventing.Messages;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.Contracts.IntegrationEventHandler", Version = "1.0")]

namespace TestMediator.Publisher.Application.IntegrationEvents.EventHandlers.IntegrationEventHandlers
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class OrderCreatedEventHandler : IIntegrationEventHandler<OrderCreatedEvent>
    {
        private readonly ISender _sender;

        [IntentManaged(Mode.Merge)]
        public OrderCreatedEventHandler(ISender sender)
        {
            _sender = sender;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task HandleAsync(OrderCreatedEvent message, CancellationToken cancellationToken = default)
        {
            await _sender.Send(new MarkOrderShippedCommand(message.OrderId), cancellationToken);
        }
    }
}
