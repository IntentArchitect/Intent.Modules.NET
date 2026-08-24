using Intent.RoslynWeaver.Attributes;
using WolverineEventing.Publish.RabbitMQ.Eventing.Messages;
using WolverineEventing.Subscribe.RabbitMQ.Application.Common.Eventing;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.Contracts.IntegrationEventHandler", Version = "1.0")]

namespace WolverineEventing.Subscribe.RabbitMQ.Application.IntegrationEvents.EventHandlers
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class ProcessOrderCommandHandler : IIntegrationEventHandler<ProcessOrderCommand>
    {
        [IntentManaged(Mode.Merge)]
        public ProcessOrderCommandHandler()
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public async Task HandleAsync(ProcessOrderCommand message, CancellationToken cancellationToken = default)
        {
            // TODO: Implement HandleAsync (ProcessOrderCommandHandler) functionality
            throw new NotImplementedException("Implement your handler logic here...");
        }
    }
}