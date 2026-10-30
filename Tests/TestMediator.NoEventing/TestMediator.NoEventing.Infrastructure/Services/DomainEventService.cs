using Intent.RoslynWeaver.Attributes;
using Mediator;
using Microsoft.Extensions.Logging;
using TestMediator.NoEventing.Application.Common.Interfaces;
using TestMediator.NoEventing.Application.Common.Models;
using TestMediator.NoEventing.Domain.Common;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Mediator.DomainEvents.DomainEventService", Version = "1.0")]

namespace TestMediator.NoEventing.Infrastructure.Services
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