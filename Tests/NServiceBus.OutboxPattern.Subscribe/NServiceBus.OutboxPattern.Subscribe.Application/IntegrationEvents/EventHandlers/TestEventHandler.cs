using Intent.RoslynWeaver.Attributes;
using NServiceBus.OutboxPattern.Publish.Eventing.Messages;
using NServiceBus.OutboxPattern.Subscribe.Application.Common.Eventing;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.Contracts.IntegrationEventHandler", Version = "1.0")]

namespace NServiceBus.OutboxPattern.Subscribe.Application.IntegrationEvents.EventHandlers
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class TestEventHandler : IIntegrationEventHandler<TestEvent>
    {
        [IntentManaged(Mode.Merge)]
        public TestEventHandler()
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public async Task HandleAsync(TestEvent message, CancellationToken cancellationToken = default)
        {
            // TODO: Implement HandleAsync (TestEventHandler) functionality
            throw new NotImplementedException("Implement your handler logic here...");
        }
    }
}