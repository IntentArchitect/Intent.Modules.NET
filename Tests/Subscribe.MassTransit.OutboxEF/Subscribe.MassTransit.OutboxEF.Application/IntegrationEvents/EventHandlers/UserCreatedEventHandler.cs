using System;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MassTransit.Messages.Shared;
using Microsoft.Extensions.Logging;
using Subscribe.MassTransit.OutboxEF.Application.Common.Eventing;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.MassTransit.IntegrationEventHandler", Version = "1.0")]

namespace Subscribe.MassTransit.OutboxEF.Application.IntegrationEvents.EventHandlers
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class UserCreatedEventHandler : IIntegrationEventHandler<UserCreatedEvent>
    {
        private readonly ILogger<UserCreatedEventHandler> _logger;

        [IntentManaged(Mode.Merge)]
        public UserCreatedEventHandler(ILogger<UserCreatedEventHandler> logger)
        {
            _logger = logger;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task HandleAsync(UserCreatedEvent message, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Received message: {Message}", message);
        }
    }
}