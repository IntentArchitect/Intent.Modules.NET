using System;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MassTransit.Messages.Shared.Orders;
using Microsoft.Extensions.Logging;
using Subscribe.MassTransit.OutboxEF.Application.Common.Eventing;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.Contracts.IntegrationEventHandler", Version = "1.0")]

namespace Subscribe.MassTransit.OutboxEF.Application.IntegrationEvents.EventHandlers
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class OrderDeletedEventHandler : IIntegrationEventHandler<OrderDeletedEvent>
    {
        private readonly ILogger<OrderDeletedEventHandler> _logger;

        [IntentManaged(Mode.Merge)]
        public OrderDeletedEventHandler(ILogger<OrderDeletedEventHandler> logger)
        {
            _logger = logger;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public async Task HandleAsync(OrderDeletedEvent message, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Received message: {Message}", message);
        }
    }
}