using System;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MassTransit.Messages.Shared;
using Microsoft.Extensions.Logging;
using Subscribe.MassTransit.OutboxMemory.Application.Common.Eventing;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.MassTransit.IntegrationEventHandlerImplementation", Version = "1.0")]

namespace Subscribe.MassTransit.OutboxMemory.Application.IntegrationEventHandlers
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class RoleDeletedEventHandler : IIntegrationEventHandler<RoleDeletedEvent>
    {
        private readonly ILogger<RoleDeletedEventHandler> _logger;

        [IntentManaged(Mode.Ignore)]
        public RoleDeletedEventHandler(ILogger<RoleDeletedEventHandler> logger)
        {
            _logger = logger;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task HandleAsync(RoleDeletedEvent message, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Received message: {Message}", message);
        }
    }
}