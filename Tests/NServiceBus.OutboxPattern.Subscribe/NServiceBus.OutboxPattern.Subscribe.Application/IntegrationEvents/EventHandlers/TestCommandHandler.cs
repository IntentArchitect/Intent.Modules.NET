using Intent.RoslynWeaver.Attributes;
using NServiceBus.OutboxPattern.Publish.Eventing.Messages;
using NServiceBus.OutboxPattern.Subscribe.Application.Common.Eventing;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.Contracts.IntegrationEventHandler", Version = "1.0")]

namespace NServiceBus.OutboxPattern.Subscribe.Application.IntegrationEvents.EventHandlers
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class TestCommandHandler : IIntegrationEventHandler<TestCommand>
    {
        [IntentManaged(Mode.Merge)]
        public TestCommandHandler()
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public async Task HandleAsync(TestCommand message, CancellationToken cancellationToken = default)
        {
            // TODO: Implement HandleAsync (TestCommandHandler) functionality
            throw new NotImplementedException("Implement your handler logic here...");
        }
    }
}