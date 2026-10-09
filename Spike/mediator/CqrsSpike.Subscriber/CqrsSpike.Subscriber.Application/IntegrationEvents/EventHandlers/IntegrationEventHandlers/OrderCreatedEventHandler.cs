using CqrsSpike.Publisher.Eventing.Messages;
using CqrsSpike.Subscriber.Application.Common.Eventing;
using CqrsSpike.Subscriber.Application.OrderRecords.RecordOrder;
using Intent.RoslynWeaver.Attributes;
using Mediator;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.Contracts.IntegrationEventHandler", Version = "1.0")]

namespace CqrsSpike.Subscriber.Application.IntegrationEvents.EventHandlers.IntegrationEventHandlers
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class OrderCreatedEventHandler : IIntegrationEventHandler<OrderCreatedEvent>
    {
        private readonly ISender _mediator;

        [IntentManaged(Mode.Merge)]
        public OrderCreatedEventHandler(ISender mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task HandleAsync(OrderCreatedEvent message, CancellationToken cancellationToken = default)
        {
            var command = new RecordOrderCommand(
                orderId: message.OrderId,
                customerName: message.CustomerName,
                total: message.Total);
            await _mediator.Send(command, cancellationToken);
        }
    }
}