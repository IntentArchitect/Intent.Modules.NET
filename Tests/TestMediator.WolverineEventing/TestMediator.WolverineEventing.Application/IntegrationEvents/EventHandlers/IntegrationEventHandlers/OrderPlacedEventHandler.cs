using Intent.RoslynWeaver.Attributes;
using Mediator;
using TestMediator.WolverineEventing.Application.Common.Eventing;
using TestMediator.WolverineEventing.Application.Orders.AcceptOrder;
using TestMediator.WolverineEventing.Eventing.Messages;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.Contracts.IntegrationEventHandler", Version = "1.0")]

namespace TestMediator.WolverineEventing.Application.IntegrationEvents.EventHandlers.IntegrationEventHandlers
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

            // Test hook: fail after the nested command has been dispatched, to show what happens to its publish.
            if (message.CustomerName == "FailAfterDispatch")
            {
                throw new InvalidOperationException("Injected failure after the nested command was dispatched.");
            }
        }
    }
}
