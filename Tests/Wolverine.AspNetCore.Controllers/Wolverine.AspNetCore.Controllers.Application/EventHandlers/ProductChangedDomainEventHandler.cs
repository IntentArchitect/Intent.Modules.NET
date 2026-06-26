using Intent.RoslynWeaver.Attributes;
using Microsoft.Extensions.Logging;
using Wolverine.AspNetCore.Controllers.Domain.Events;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Wolverine.DomainEvents.DomainEventHandler", Version = "1.0")]

namespace Wolverine.AspNetCore.Controllers.Application.EventHandlers
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class ProductChangedDomainEventHandler
    {
        private readonly ILogger<ProductChangedDomainEventHandler> _logger;

        [IntentManaged(Mode.Merge)]
        public ProductChangedDomainEventHandler(ILogger<ProductChangedDomainEventHandler> logger)
        {
            _logger = logger;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public async Task Handle(ProductChangedDomainEvent domainEvent, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling product changed domain event: {Event}", domainEvent);
        }
    }
}