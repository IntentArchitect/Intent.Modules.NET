using CqrsSpike.Publisher.Application.Common.Interfaces;
using CqrsSpike.Publisher.Application.Common.Models;
using CqrsSpike.Publisher.Domain.Common;
using Intent.RoslynWeaver.Attributes;
using Mediator;
using Microsoft.Extensions.Logging;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Mediator.DomainEvents.DomainEventService", Version = "2.0")]

namespace CqrsSpike.Publisher.Infrastructure.Services
{
    public class DomainEventService : IDomainEventService
    {
        private readonly ILogger<DomainEventService> _logger;
        private readonly IPublisher _mediator;

        public DomainEventService(ILogger<DomainEventService> logger, IPublisher mediator)
        {
            _logger = logger;
            _mediator = mediator;
        }

        public async Task Publish(DomainEvent domainEvent, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Publishing domain event. Event - {event}", domainEvent.GetType().Name);
            var notification = DomainEventNotificationFactory.Create(domainEvent);
            if (notification is null)
            {
                _logger.LogDebug("No notification is mapped for domain event {event}", domainEvent.GetType().Name);
                return;
            }

            await _mediator.Publish(notification, cancellationToken);
        }
    }
}