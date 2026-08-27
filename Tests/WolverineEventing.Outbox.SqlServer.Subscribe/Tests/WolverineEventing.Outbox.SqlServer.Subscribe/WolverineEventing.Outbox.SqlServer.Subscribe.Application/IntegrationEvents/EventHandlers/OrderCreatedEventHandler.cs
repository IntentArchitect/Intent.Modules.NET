using Intent.RoslynWeaver.Attributes;
using WolverineEventing.Outbox.SqlServer.Subscribe.Application.Common.Eventing;
using WolverineEventing.Outbox.SqlServer.Subscribe.Eventing.Messages;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.Contracts.IntegrationEventHandler", Version = "1.0")]

namespace WolverineEventing.Outbox.SqlServer.Subscribe.Application.IntegrationEvents.EventHandlers
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class OrderCreatedEventHandler : IIntegrationEventHandler<OrderCreatedEvent>
    {
        [IntentManaged(Mode.Merge)]
        public OrderCreatedEventHandler()
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public async Task HandleAsync(OrderCreatedEvent message, CancellationToken cancellationToken = default)
        {
            // TODO: Implement HandleAsync (OrderCreatedEventHandler) functionality
            throw new NotImplementedException("Implement your handler logic here...");
        }
    }
}