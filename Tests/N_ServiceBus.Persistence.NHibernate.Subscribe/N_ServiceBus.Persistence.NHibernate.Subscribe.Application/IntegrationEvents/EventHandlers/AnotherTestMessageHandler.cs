using Intent.RoslynWeaver.Attributes;
using N_ServiceBus.Persistence.NHibernate.Subscribe.Application.Common.Eventing;
using N_ServiceBus.Persistence.NHibernate.Subscribe.Eventing.Messages;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.Contracts.IntegrationEventHandler", Version = "1.0")]

namespace N_ServiceBus.Persistence.NHibernate.Subscribe.Application.IntegrationEvents.EventHandlers
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
            Console.WriteLine($"[HANDLER HIT] Subscribe.AnotherTestMessageHandler received: {message.Message}");
        }
    }
}