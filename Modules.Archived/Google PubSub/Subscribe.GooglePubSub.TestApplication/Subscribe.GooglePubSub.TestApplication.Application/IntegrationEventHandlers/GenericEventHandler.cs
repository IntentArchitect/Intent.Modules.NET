using System;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using Subscribe.GooglePubSub.TestApplication.Application.Common.Eventing;
using Subscribe.GooglePubSub.TestApplication.Application.IntegrationEvents;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.GoogleCloud.PubSub.EventingTemplates.GenericIntegrationEventHandlerImplementation", Version = "1.0")]

namespace Subscribe.GooglePubSub.TestApplication.Application.IntegrationEventHandlers
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GenericEventHandler : IIntegrationEventHandler<GenericMessage>
    {
        [IntentManaged(Mode.Ignore)]
        public GenericEventHandler()
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task HandleAsync(GenericMessage message, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}