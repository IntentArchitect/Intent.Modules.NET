using System;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.ServiceModelling.ComplexTypes.Application.Common.Interfaces;
using CleanArchitecture.ServiceModelling.ComplexTypes.Application.Common.Models;
using CleanArchitecture.ServiceModelling.ComplexTypes.Domain.Common;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using Microsoft.Extensions.Logging;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.MediatR.DomainEvents.DomainEventService", Version = "2.0")]

namespace CleanArchitecture.ServiceModelling.ComplexTypes.Infrastructure.Services
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
            await _mediator.Publish(GetNotificationCorrespondingToDomainEvent(domainEvent), cancellationToken);
        }

        private static INotification GetNotificationCorrespondingToDomainEvent(DomainEvent domainEvent)
        {
            var result = Activator.CreateInstance(
                typeof(DomainEventNotification<>).MakeGenericType(domainEvent.GetType()), domainEvent);

            return result == null
                ? throw new Exception($"Unable to create DomainEventNotification<{domainEvent.GetType().Name}>")
                : (INotification)result;
        }
    }
}