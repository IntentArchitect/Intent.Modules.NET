using System;
using System.Threading;
using System.Threading.Tasks;
using AzureFunctions.AzureServiceBus.Application.Common.Eventing;
using AzureFunctions.AzureServiceBus.GroupA.Eventing.Messages;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Extensions.Logging;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.Contracts.IntegrationEventHandler", Version = "1.0")]

namespace AzureFunctions.AzureServiceBus.Application.IntegrationEvents.EventHandlers
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
            _logger.LogInformation("ClientCreatedEvent : {Message}", message);
        }
    }
}