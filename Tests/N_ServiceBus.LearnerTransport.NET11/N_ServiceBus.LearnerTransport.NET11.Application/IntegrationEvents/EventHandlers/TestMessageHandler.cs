using Intent.RoslynWeaver.Attributes;
using N_ServiceBus.LearnerTransport.NET11.Application.Common.Eventing;
using N_ServiceBus.LearnerTransport.NET11.Eventing.Messages;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.Contracts.IntegrationEventHandler", Version = "1.0")]

namespace N_ServiceBus.LearnerTransport.NET11.Application.IntegrationEvents.EventHandlers
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class TestMessageHandler : IIntegrationEventHandler<TestMessageEvent>
    {
        [IntentManaged(Mode.Merge)]
        public TestMessageHandler()
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task HandleAsync(TestMessageEvent message, CancellationToken cancellationToken = default)
        {
            Console.WriteLine($"[HANDLER HIT] TestMessageHandler received: {message.Message}");
        }
    }
}