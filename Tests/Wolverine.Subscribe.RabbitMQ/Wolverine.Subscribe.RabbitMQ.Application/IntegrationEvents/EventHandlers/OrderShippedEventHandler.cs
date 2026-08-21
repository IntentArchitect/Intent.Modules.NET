using Intent.RoslynWeaver.Attributes;
using Wolverine.Publish.RabbitMQ.Eventing.Messages;
using Wolverine.Subscribe.RabbitMQ.Application.Common.Eventing;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.Contracts.IntegrationEventHandler", Version = "1.0")]

namespace Wolverine.Subscribe.RabbitMQ.Application.IntegrationEvents.EventHandlers
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class OrderShippedEventHandler : IIntegrationEventHandler<OrderShippedEvent>
    {
        [IntentManaged(Mode.Merge)]
        public OrderShippedEventHandler()
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public async Task HandleAsync(OrderShippedEvent message, CancellationToken cancellationToken = default)
        {
            // TODO: Implement HandleAsync (OrderShippedEventHandler) functionality
            throw new NotImplementedException("Implement your handler logic here...");
        }
    }
}