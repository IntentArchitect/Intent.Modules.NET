using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Extensions.Logging;
using Wolverine;
using Wolverine.AzureFunctions.Application.Common.Interfaces;
using Wolverine.AzureFunctions.Domain.Common;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Wolverine.DomainEvents.DomainEventService", Version = "1.0")]

namespace Wolverine.AzureFunctions.Infrastructure.Services
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