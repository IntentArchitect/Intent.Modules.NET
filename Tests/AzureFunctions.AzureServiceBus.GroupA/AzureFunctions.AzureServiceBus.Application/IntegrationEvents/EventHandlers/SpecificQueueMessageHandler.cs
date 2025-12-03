using System;
using System.Threading;
using System.Threading.Tasks;
using AzureFunctions.AzureServiceBus.Application.Common.Eventing;
using AzureFunctions.AzureServiceBus.GroupB.Eventing.Messages;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Extensions.Logging;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.Contracts.IntegrationEventHandler", Version = "1.0")]

namespace AzureFunctions.AzureServiceBus.Application.IntegrationEvents.EventHandlers
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class SpecificQueueMessageHandler : IIntegrationEventHandler<SpecificQueueOneMessageEvent>, IIntegrationEventHandler<SpecificQueueTwoMessageEvent>
    {
        private readonly ILogger<SpecificQueueMessageHandler> _logger;

        [IntentManaged(Mode.Merge)]
        public SpecificQueueMessageHandler(ILogger<SpecificQueueMessageHandler> logger)
        {
            _logger = logger;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public async Task HandleAsync(SpecificQueueOneMessageEvent message, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("SpecificQueueOneMessageEvent : {Message}", message);
        }

        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public async Task HandleAsync(SpecificQueueTwoMessageEvent message, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("SpecificQueueTwoMessageEvent : {Message}", message);
        }
    }
}