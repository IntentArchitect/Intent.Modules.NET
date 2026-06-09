using Intent.RoslynWeaver.Attributes;
using NServiceBus.OutboxPattern.Subscribe.Application.Common.Eventing;
using NServiceBus.OutboxPattern.Subscribe.Eventing.Messages;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.Contracts.IntegrationEventHandler", Version = "1.0")]

namespace NServiceBus.OutboxPattern.Subscribe.Application.IntegrationEvents.EventHandlers
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class AnotherTestMessageHandler : IIntegrationEventHandler<AnotherTestMessageEvent>
    {
        [IntentManaged(Mode.Merge)]
        public AnotherTestMessageHandler()
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public async Task HandleAsync(AnotherTestMessageEvent message, CancellationToken cancellationToken = default)
        {
            
        }
    }
}