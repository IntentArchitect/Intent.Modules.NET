using Intent.RoslynWeaver.Attributes;
using NServiceBus.AzureServiceBus.Application.Common.Eventing;
using NServiceBus.AzureServiceBus.Eventing.Messages;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.Contracts.IntegrationEventHandler", Version = "1.0")]

namespace NServiceBus.AzureServiceBus.Application.IntegrationEvents.EventHandlers
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class TestMessageHandler : IIntegrationEventHandler<TestMessageEvent>
    {
        [IntentManaged(Mode.Merge)]
        public TestMessageHandler()
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public async Task HandleAsync(TestMessageEvent message, CancellationToken cancellationToken = default)
        {
            // TODO: Implement HandleAsync (TestMessageHandler) functionality
            throw new NotImplementedException("Implement your handler logic here...");
        }
    }
}