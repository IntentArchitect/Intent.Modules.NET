using Intent.RoslynWeaver.Attributes;
using NServiceBus.OutboxPattern.Publish.Eventing.Messages;
using NServiceBus.OutboxPattern.Subscribe.Application.Common.Eventing;
using NServiceBus.OutboxPattern.Subscribe.Eventing.Messages;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.Contracts.IntegrationEventHandler", Version = "1.0")]

namespace NServiceBus.OutboxPattern.Subscribe.Application.IntegrationEvents.EventHandlers
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