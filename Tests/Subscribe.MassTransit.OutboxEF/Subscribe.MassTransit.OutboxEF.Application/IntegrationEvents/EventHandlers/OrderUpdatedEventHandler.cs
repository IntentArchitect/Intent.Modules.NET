using System;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MassTransit.Messages.Shared.Orders;
using Microsoft.Extensions.Logging;
using Subscribe.MassTransit.OutboxEF.Application.Common.Eventing;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.MassTransit.IntegrationEventHandler", Version = "1.0")]

namespace Subscribe.MassTransit.OutboxEF.Application.IntegrationEvents.EventHandlers
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class OrderUpdatedEventHandler : IIntegrationEventHandler<OrderUpdatedEvent>
    {
        private readonly ILogger<OrderUpdatedEventHandler> _logger;

        [IntentManaged(Mode.Merge)]
        public OrderUpdatedEventHandler(ILogger<OrderUpdatedEventHandler> logger)
        {
            _logger = logger;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task HandleAsync(OrderUpdatedEvent message, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Received message: {Message}", message);
        }
    }
}