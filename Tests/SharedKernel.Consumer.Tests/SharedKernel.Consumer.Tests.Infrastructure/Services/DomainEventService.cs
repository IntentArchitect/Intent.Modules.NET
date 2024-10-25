using System;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using Microsoft.Extensions.Logging;
using SharedKernel.Consumer.Tests.Application.Common.Interfaces;
using SharedKernel.Consumer.Tests.Application.Common.Models;
using SharedKernel.Consumer.Tests.Domain.Common;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.MediatR.DomainEvents.DomainEventService", Version = "2.0")]

namespace SharedKernel.Consumer.Tests.Infrastructure.Services
{
    public class DomainEventService : IDomainEventService, SharedKernel.Kernel.Tests.Application.Common.Interfaces.IDomainEventService
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

        private INotification GetNotificationCorrespondingToDomainEvent(DomainEvent domainEvent)
        {
            var result = Activator.CreateInstance(
                typeof(DomainEventNotification<>).MakeGenericType(domainEvent.GetType()), domainEvent);
            if (result == null)
            {
                throw new Exception($"Unable to create DomainEventNotification<{domainEvent.GetType().Name}>");
            }

            return (INotification)result;
        }

        public async Task Publish(
            SharedKernel.Kernel.Tests.Domain.Common.DomainEvent domainEvent,
            CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Publishing domain event. Event - {event}", domainEvent.GetType().Name);
            await _mediator.Publish(GetNotificationCorrespondingToDomainEvent(domainEvent), cancellationToken);
        }

        private INotification GetNotificationCorrespondingToDomainEvent(SharedKernel.Kernel.Tests.Domain.Common.DomainEvent domainEvent)
        {
            var result = Activator.CreateInstance(
                typeof(SharedKernel.Kernel.Tests.Application.Common.Models.DomainEventNotification<>).MakeGenericType(domainEvent.GetType()), domainEvent);
            if (result == null)
                throw new Exception($"Unable to create DomainEventNotification<{domainEvent.GetType().Name}>");
            return (INotification)result;
        }
    }
}