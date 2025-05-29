using System;
using System.Threading;
using System.Threading.Tasks;
using AzureFunctions.AzureServiceBus.Application.Common.Eventing;
using AzureFunctions.AzureServiceBus.GroupA.Eventing.Messages;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Extensions.Logging;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.AzureServiceBus.IntegrationEventHandler", Version = "1.0")]

namespace AzureFunctions.AzureServiceBus.Application.IntegrationEvents.EventHandlers
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class CreateOrgIntegrationCommandHandler : IIntegrationEventHandler<CreateOrgIntegrationCommand>
    {
        private readonly ILogger<CreateOrgIntegrationCommandHandler> _logger;

        [IntentManaged(Mode.Merge)]
        public CreateOrgIntegrationCommandHandler(ILogger<CreateOrgIntegrationCommandHandler> logger)
        {
            _logger = logger;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public async Task HandleAsync(CreateOrgIntegrationCommand message, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("CreateOrgIntegrationCommand : {Message}", message);
        }
    }
}