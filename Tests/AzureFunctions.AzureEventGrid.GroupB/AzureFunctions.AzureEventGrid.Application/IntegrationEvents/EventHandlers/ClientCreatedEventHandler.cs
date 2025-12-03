using System;
using System.Threading;
using System.Threading.Tasks;
using AzureFunctions.AzureEventGrid.Application.Common.Eventing;
using AzureFunctions.AzureEventGrid.GroupA.Eventing.Messages;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Extensions.Logging;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.Contracts.IntegrationEventHandler", Version = "1.0")]

namespace AzureFunctions.AzureEventGrid.Application.IntegrationEvents.EventHandlers
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class ClientCreatedEventHandler : IIntegrationEventHandler<ClientCreatedEvent>
    {
        private readonly ILogger<ClientCreatedEventHandler> _logger;
        private readonly IEventContext _eventContext;

        [IntentManaged(Mode.Merge)]
        public ClientCreatedEventHandler(ILogger<ClientCreatedEventHandler> logger, IEventContext eventContext)
        {
            _logger = logger;
            _eventContext = eventContext;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public async Task HandleAsync(ClientCreatedEvent message, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Received: {Message}, Data: {Data}", message, _eventContext.AdditionalData);
        }
    }
}