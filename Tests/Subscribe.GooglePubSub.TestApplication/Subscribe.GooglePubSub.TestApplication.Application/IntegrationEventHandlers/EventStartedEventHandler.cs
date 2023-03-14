using System;
using System.Threading;
using System.Threading.Tasks;
using Eventing;
using Intent.RoslynWeaver.Attributes;
using Subscribe.GooglePubSub.TestApplication.Application.Common.Eventing;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.GoogleCloud.PubSub.EventingTemplates.IntegrationEventHandlerImplementation", Version = "1.0")]

namespace Subscribe.GooglePubSub.TestApplication.Application.IntegrationEventHandlers
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