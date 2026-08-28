using Intent.RoslynWeaver.Attributes;
using Microsoft.Extensions.Logging;
using WolverineEventing.ErrorPolicy.RetryWithCooldown.Application.Common.Eventing;
using WolverineEventing.ErrorPolicy.RetryWithCooldown.Eventing.Messages;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.Contracts.IntegrationEventHandler", Version = "1.0")]

namespace WolverineEventing.ErrorPolicy.RetryWithCooldown.Application.IntegrationEvents.EventHandlers
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
            // Throws on every delivery, on purpose. This is what makes Error Handling Policy =
            // Retry with cooldown observable: retries on the configured increasing delays, then
            // the error queue. Logged before throwing so each delivery attempt is timestamped,
            // which is what makes the cooldown cadence measurable rather than inferred.
            _logger.LogWarning("ATTEMPT OrderCreatedEvent OrderId={OrderId}", message.OrderId);

            await Task.CompletedTask;

            throw new InvalidOperationException(
                $"Deliberate failure for OrderId {message.OrderId}, exercising the Error Handling Policy.");
        }
    }
}