using Intent.RoslynWeaver.Attributes;
using Microsoft.Extensions.Logging;
using WolverineEventing.Publish.RabbitMQ.Eventing.Messages;
using WolverineEventing.Subscribe.RabbitMQ.Application.Common.Eventing;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.Contracts.IntegrationEventHandler", Version = "1.0")]

namespace WolverineEventing.Subscribe.RabbitMQ.Application.IntegrationEvents.EventHandlers
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class ProcessOrderCommandHandler : IIntegrationEventHandler<ProcessOrderCommand>
    {
        private readonly ILogger<ProcessOrderCommandHandler> _logger;

        [IntentManaged(Mode.Merge)]
        public ProcessOrderCommandHandler(ILogger<ProcessOrderCommandHandler> logger)
        {
            _logger = logger;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public async Task HandleAsync(ProcessOrderCommand message, CancellationToken cancellationToken = default)
        {
            // Hand-written body. Logs the payload rather than throwing, so a runtime check can prove
            // the DATA arrived intact and not merely that the handler was reached.
            _logger.LogInformation(
                "HANDLED ProcessOrderCommand OrderId={OrderId}",
                message.OrderId);

            await Task.CompletedTask;
        }
    }
}
