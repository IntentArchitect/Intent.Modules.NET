using System;
using System.Threading;
using System.Threading.Tasks;
using Eventing;
using Intent.RoslynWeaver.Attributes;
using Subscribe.MassTransit.TestApplication.Application.Common.Eventing;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.MassTransit.IntegrationEventHandlerImplementation", Version = "1.0")]

namespace Subscribe.MassTransit.TestApplication.Application.IntegrationEventHandlers
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class EventStartedEventHandler : IIntegrationEventHandler<EventStartedEvent>
    {
        [IntentManaged(Mode.Ignore)]
        public EventStartedEventHandler()
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task HandleAsync(EventStartedEvent message, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}