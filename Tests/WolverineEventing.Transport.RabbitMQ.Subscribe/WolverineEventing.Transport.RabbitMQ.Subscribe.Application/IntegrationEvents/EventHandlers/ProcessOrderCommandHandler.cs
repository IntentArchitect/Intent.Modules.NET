using Intent.RoslynWeaver.Attributes;
using WolverineEventing.Transport.RabbitMQ.Subscribe.Application.Common.Eventing;
using WolverineEventing.Transport.RabbitMQ.Subscribe.Eventing.Messages;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.Contracts.IntegrationEventHandler", Version = "1.0")]

namespace WolverineEventing.Transport.RabbitMQ.Subscribe.Application.IntegrationEvents.EventHandlers
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