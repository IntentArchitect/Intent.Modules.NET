using Intent.RoslynWeaver.Attributes;
using N_ServiceBus.OutboxPattern.Publish.Eventing.Messages;
using N_ServiceBus.OutboxPattern.Subscribe.Application.Common.Eventing;
using N_ServiceBus.OutboxPattern.Subscribe.Eventing.Messages;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.Contracts.IntegrationEventHandler", Version = "1.0")]

namespace N_ServiceBus.OutboxPattern.Subscribe.Application.IntegrationEvents.EventHandlers
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class TestEventHandler : IIntegrationEventHandler<TestEvent>
    {
        private readonly IMessageBus _messageBus;
        [IntentManaged(Mode.Merge)]
        public TestEventHandler(IMessageBus messageBus)
        {
            _messageBus = messageBus;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task HandleAsync(TestEvent message, CancellationToken cancellationToken = default)
        {
            _messageBus.Publish(new AnotherTestMessageEvent
            {
                Message = message.Message
            });
        }
    }
}