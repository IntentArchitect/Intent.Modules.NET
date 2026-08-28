using Intent.RoslynWeaver.Attributes;
using Microsoft.Extensions.Logging;
using WolverineEventing.Outbox.SqlServer.Publish.Eventing.Messages;
using WolverineEventing.Outbox.SqlServer.Subscribe.Application.Common.Eventing;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.Contracts.IntegrationEventHandler", Version = "1.0")]

namespace WolverineEventing.Outbox.SqlServer.Subscribe.Application.IntegrationEvents.EventHandlers
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class OrderCreatedEventHandler : IIntegrationEventHandler<OrderCreatedEvent>
    {
        private readonly ILogger<OrderCreatedEventHandler> _logger;

        [IntentManaged(Mode.Merge)]
        public OrderCreatedEventHandler(ILogger<OrderCreatedEventHandler> logger)
        {
            _logger = logger;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public async Task HandleAsync(OrderCreatedEvent message, CancellationToken cancellationToken = default)
        {
            // Hand-written body. Logs the payload rather than throwing, so a runtime check can prove
            // the DATA arrived intact and not merely that the handler was reached.
            _logger.LogInformation(
                "HANDLED OrderCreatedEvent OrderId={OrderId}",
                message.OrderId);

            await Task.CompletedTask;
        }
    }
}