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
    public class BasketUpdatedEventHandler : IIntegrationEventHandler<BasketUpdatedEvent>
    {
        private readonly ILogger<BasketUpdatedEventHandler> _logger;

        [IntentManaged(Mode.Merge)]
        public BasketUpdatedEventHandler(ILogger<BasketUpdatedEventHandler> logger)
        {
            _logger = logger;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public async Task HandleAsync(BasketUpdatedEvent message, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Received message: {Message}", message);
        }
    }
}