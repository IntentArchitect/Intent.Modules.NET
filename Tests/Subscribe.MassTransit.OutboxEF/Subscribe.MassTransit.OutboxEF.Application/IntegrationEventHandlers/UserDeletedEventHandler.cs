using System;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MassTransit.Messages.Shared;
using Microsoft.Extensions.Logging;
using Subscribe.MassTransit.OutboxEF.Application.Common.Eventing;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.MassTransit.IntegrationEventHandlerImplementation", Version = "1.0")]

namespace Subscribe.MassTransit.OutboxEF.Application.IntegrationEventHandlers
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class UserDeletedEventHandler : IIntegrationEventHandler<UserDeletedEvent>
    {
        private readonly ILogger<UserDeletedEventHandler> _logger;

        [IntentManaged(Mode.Ignore)]
        public UserDeletedEventHandler(ILogger<UserDeletedEventHandler> logger)
        {
            _logger = logger;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task HandleAsync(UserDeletedEvent message, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Received message: {Message}", message);
        }
    }
}