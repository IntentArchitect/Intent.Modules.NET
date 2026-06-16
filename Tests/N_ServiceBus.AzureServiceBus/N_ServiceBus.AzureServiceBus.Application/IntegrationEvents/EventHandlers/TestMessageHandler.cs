using Intent.RoslynWeaver.Attributes;
using N_ServiceBus.AzureServiceBus.Application.Common.Eventing;
using N_ServiceBus.AzureServiceBus.Eventing.Messages;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.Contracts.IntegrationEventHandler", Version = "1.0")]

namespace N_ServiceBus.AzureServiceBus.Application.IntegrationEvents.EventHandlers
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
            Console.WriteLine($"[HANDLER HIT] AzureServiceBus.TestMessageHandler received: {message.Message}");
        }
    }
}