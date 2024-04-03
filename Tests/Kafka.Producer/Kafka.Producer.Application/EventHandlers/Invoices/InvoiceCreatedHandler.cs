using System;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using Kafka.Producer.Application.Common.Eventing;
using Kafka.Producer.Application.Common.Models;
using Kafka.Producer.Domain.Events;
using Kafka.Producer.Eventing.Messages;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.MediatR.DomainEvents.DomainEventHandler", Version = "1.0")]

namespace Kafka.Producer.Application.EventHandlers.Invoices
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class InvoiceCreatedHandler : INotificationHandler<DomainEventNotification<InvoiceCreated>>, INotificationHandler<DomainEventNotification<InvoiceUpdated>>
    {
        private readonly IEventBus _eventBus;

        [IntentManaged(Mode.Merge)]
        public InvoiceCreatedHandler(IEventBus eventBus)
        {
            _eventBus = eventBus;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task Handle(DomainEventNotification<InvoiceCreated> notification, CancellationToken cancellationToken)
        {
            var domainEvent = notification.DomainEvent;
            _eventBus.Publish(new InvoiceCreatedEvent
            {
                Id = domainEvent.Invoice.Id,
                Note = domainEvent.Invoice.Note
            });
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task Handle(DomainEventNotification<InvoiceUpdated> notification, CancellationToken cancellationToken)
        {
            var domainEvent = notification.DomainEvent;
            _eventBus.Publish(new InvoiceUpdatedEvent
            {
                Id = domainEvent.Invoice.Id,
                Note = domainEvent.Invoice.Note
            });
        }
    }
}