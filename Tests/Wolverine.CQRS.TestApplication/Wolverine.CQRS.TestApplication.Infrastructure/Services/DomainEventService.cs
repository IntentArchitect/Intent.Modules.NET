using Intent.RoslynWeaver.Attributes;
using Microsoft.Extensions.Logging;
using Wolverine;
using Wolverine.CQRS.TestApplication.Application.Common.Interfaces;
using Wolverine.CQRS.TestApplication.Domain.Common;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.MediatR.DomainEvents.DomainEventService", Version = "2.0")]

namespace Wolverine.CQRS.TestApplication.Infrastructure.Services
{
    public class DomainEventService : IDomainEventService
    {
        private readonly ILogger<DomainEventService> _logger;
        private readonly IMessageBus _messageBus;

        public DomainEventService(ILogger<DomainEventService> logger, IMessageBus messageBus)
        {
            _logger = logger;
            _messageBus = messageBus;
        }

        public async Task Publish(DomainEvent domainEvent, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Publishing domain event. Event - {event}", domainEvent.GetType().Name);
            await _messageBus.PublishAsync(domainEvent);
        }
    }
}
