using System;
using System.Threading;
using System.Threading.Tasks;
using AzureFunctions.AzureEventGrid.Application.Common.Eventing;
using AzureFunctions.AzureEventGrid.Eventing.Messages;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Extensions.Logging;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.AzureEventGrid.IntegrationEventHandler", Version = "1.0")]

namespace AzureFunctions.AzureEventGrid.Application.IntegrationEvents.EventHandlers
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class ClientCreatedEventHandler : IIntegrationEventHandler<ClientCreatedEvent>
    {
        private readonly ILogger<ClientCreatedEventHandler> _logger;

        [IntentManaged(Mode.Merge)]
        public ClientCreatedEventHandler(ILogger<ClientCreatedEventHandler> logger)
        {
            _logger = logger;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public async Task HandleAsync(ClientCreatedEvent message, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Received: {Message}", message);
        }
    }
}