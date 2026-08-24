using Intent.RoslynWeaver.Attributes;
using Microsoft.Extensions.Logging;
using WolverineEventing.Publish.RabbitMQ.Eventing.Messages;
using WolverineEventing.Subscribe.RabbitMQ.Application.Common.Eventing;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.Contracts.IntegrationEventHandler", Version = "1.0")]

namespace WolverineEventing.Subscribe.RabbitMQ.Application.IntegrationEvents.EventHandlers
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class FailingOrderEventHandler : IIntegrationEventHandler<FailingOrderEvent>
    {
        private readonly ILogger<FailingOrderEventHandler> _logger;

        [IntentManaged(Mode.Merge)]
        public FailingOrderEventHandler(ILogger<FailingOrderEventHandler> logger)
        {
            _logger = logger;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public async Task HandleAsync(FailingOrderEvent message, CancellationToken cancellationToken = default)
        {
            // Throws on every delivery, on purpose. This is the retry probe: it makes the Error
            // Handling Policy observable - retry with cooldown on the configured delays, then the
            // Error Queue - without the happy-path handlers having to fail. Never make this
            // succeed. Kept inside the body because the body is Mode.Merge and a doc comment above
            // the signature is stripped on every Software Factory run.
            // Logged before throwing so each delivery attempt is timestamped in the log, which is
            // what makes the retry cadence measurable rather than inferred.
            _logger.LogWarning("ATTEMPT FailingOrderEvent OrderId={OrderId}", message.OrderId);

            await Task.CompletedTask;

            throw new InvalidOperationException(
                $"Deliberate failure for OrderId {message.OrderId}, exercising the Error Handling Policy.");
        }
    }
}
