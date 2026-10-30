using Intent.RoslynWeaver.Attributes;
using Mediator;
using TestMediator.WolverineSubscribeOnly.Application.Common.Eventing;
using TestMediator.WolverineSubscribeOnly.Application.Orders.RecordOrder;
using TestMediator.WolverineSubscribeOnly.Eventing.Messages;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.Contracts.IntegrationEventHandler", Version = "1.0")]

namespace TestMediator.WolverineSubscribeOnly.Application.IntegrationEvents.EventHandlers.IntegrationEventHandlers
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
            await _sender.Send(new RecordOrderCommand(message.OrderId, message.CustomerName), cancellationToken);
        }
    }
}