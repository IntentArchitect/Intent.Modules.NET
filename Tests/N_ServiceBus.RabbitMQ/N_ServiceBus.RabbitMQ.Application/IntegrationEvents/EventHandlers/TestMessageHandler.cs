using Intent.RoslynWeaver.Attributes;
using N_ServiceBus.RabbitMQ.Application.Common.Eventing;
using N_ServiceBus.RabbitMQ.Eventing.Messages;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.Contracts.IntegrationEventHandler", Version = "1.0")]

namespace N_ServiceBus.RabbitMQ.Application.IntegrationEvents.EventHandlers
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
            Console.WriteLine($"[HANDLER HIT] RabbitMQ.TestMessageHandler received: {message.Message}");
        }
    }
}