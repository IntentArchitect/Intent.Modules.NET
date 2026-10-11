using CqrsSpike.Publisher.Application.Common.Interfaces;
using CqrsSpike.Publisher.Application.Common.Models;
using CqrsSpike.Publisher.Domain.Common;
using Mediator;
using Microsoft.Extensions.Logging;

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
            await _mediator.Publish(DomainEventNotificationFactory.Create(domainEvent), cancellationToken);
        }
    }
}