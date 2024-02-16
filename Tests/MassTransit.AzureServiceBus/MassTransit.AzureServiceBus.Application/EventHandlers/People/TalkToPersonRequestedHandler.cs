using System;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MassTransit.AzureServiceBus.Application.Common.Eventing;
using MassTransit.AzureServiceBus.Application.Common.Models;
using MassTransit.AzureServiceBus.Domain.Events;
using MassTransit.AzureServiceBus.Services;
using MassTransit.AzureServiceBus.Services.People;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.MediatR.DomainEvents.DomainEventHandler", Version = "1.0")]

namespace MassTransit.AzureServiceBus.Application.EventHandlers.People
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class TalkToPersonRequestedHandler : INotificationHandler<DomainEventNotification<TalkToPersonRequested>>
    {
        private readonly IEventBus _eventBus;

        [IntentManaged(Mode.Merge)]
        public TalkToPersonRequestedHandler(IEventBus eventBus)
        {
            _eventBus = eventBus;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task Handle(
            DomainEventNotification<TalkToPersonRequested> notification,
            CancellationToken cancellationToken)
        {
            var domainEvent = notification.DomainEvent;
            _eventBus.Send(new TalkToPersonCommand
            {
                Message = domainEvent.Message,
                FirstName = domainEvent.FirstName,
                LastName = domainEvent.LastName
            });
        }
    }
}