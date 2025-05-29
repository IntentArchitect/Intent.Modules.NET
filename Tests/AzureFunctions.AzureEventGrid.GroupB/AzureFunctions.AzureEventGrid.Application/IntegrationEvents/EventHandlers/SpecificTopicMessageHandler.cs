using System;
using System.Threading;
using System.Threading.Tasks;
using AzureFunctions.AzureEventGrid.Application.Common.Eventing;
using AzureFunctions.AzureEventGrid.Eventing.Messages;
using AzureFunctions.AzureEventGrid.GroupB.Eventing.Messages;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Extensions.Logging;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.AzureEventGrid.IntegrationEventHandler", Version = "1.0")]

namespace AzureFunctions.AzureEventGrid.Application.IntegrationEvents.EventHandlers
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class SpecificTopicMessageHandler : IIntegrationEventHandler<SpecificTopicOneMessageEvent>, IIntegrationEventHandler<SpecificTopicTwoMessageEvent>
    {
        private readonly ILogger<SpecificTopicMessageHandler> _logger;

        [IntentManaged(Mode.Merge)]
        public SpecificTopicMessageHandler(ILogger<SpecificTopicMessageHandler> logger)
        {
            _logger = logger;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public async Task HandleAsync(SpecificTopicOneMessageEvent message, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Received: {Message}", message);
        }

        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public async Task HandleAsync(SpecificTopicTwoMessageEvent message, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Received: {Message}", message);
        }
    }
}