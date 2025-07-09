using System;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MassTransit.Messages.Shared.Baskets;
using Microsoft.Extensions.Logging;
using Subscribe.MassTransit.OutboxMemory.Application.Common.Eventing;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.MassTransit.IntegrationEventHandler", Version = "1.0")]

namespace Subscribe.MassTransit.OutboxMemory.Application.IntegrationEvents.EventHandlers
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class BasketDeletedEventHandler : IIntegrationEventHandler<BasketDeletedEvent>
    {
        private readonly ILogger<BasketDeletedEventHandler> _logger;

        [IntentManaged(Mode.Merge)]
        public BasketDeletedEventHandler(ILogger<BasketDeletedEventHandler> logger)
        {
            _logger = logger;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public async Task HandleAsync(BasketDeletedEvent message, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Received message: {Message}", message);
        }
    }
}