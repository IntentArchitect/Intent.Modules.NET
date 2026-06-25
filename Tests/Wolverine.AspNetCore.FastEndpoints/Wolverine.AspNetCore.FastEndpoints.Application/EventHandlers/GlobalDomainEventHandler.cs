using Intent.RoslynWeaver.Attributes;
using Microsoft.Extensions.Logging;
using Wolverine.AspNetCore.FastEndpoints.Domain.Events;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Wolverine.DomainEvents.DefaultDomainEventHandler", Version = "1.0")]

namespace Wolverine.AspNetCore.FastEndpoints.Application.EventHandlers
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GlobalDomainEventHandler
    {
        private readonly ILogger<GlobalDomainEventHandler> _logger;

        [IntentManaged(Mode.Merge)]
        public GlobalDomainEventHandler(ILogger<GlobalDomainEventHandler> logger)
        {
            _logger = logger;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public async Task Handle(GlobalDomainEvent domainEvent, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling global domain event: {Event}", domainEvent);
        }
    }
}