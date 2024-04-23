using System;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Extensions.Logging;
using Subscribe.GooglePubSub.TestApplication.Application.Common.Eventing;
using Subscribe.GooglePubSub.TestApplication.Eventing.Messages;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.GoogleCloud.PubSub.EventingTemplates.IntegrationEventHandlerImplementation", Version = "1.0")]

namespace Subscribe.GooglePubSub.TestApplication.Application.IntegrationEventHandlers
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class EventStartedEventHandler : IIntegrationEventHandler<EventStartedEvent>
    {
        private readonly ILogger<EventStartedEventHandler> _logger;

        [IntentManaged(Mode.Ignore)]
        public EventStartedEventHandler(ILogger<EventStartedEventHandler> logger)
        {
            _logger = logger;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task HandleAsync(EventStartedEvent message, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation($"EventStarted received - {message.Message}");
        }
    }
}