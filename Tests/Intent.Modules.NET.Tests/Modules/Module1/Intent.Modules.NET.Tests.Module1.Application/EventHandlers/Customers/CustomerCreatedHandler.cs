using Intent.Modules.NET.Tests.Application.Core.Common.Eventing;
using Intent.Modules.NET.Tests.Module1.Application.Common.Models;
using Intent.Modules.NET.Tests.Module1.Domain.Events;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using Module1.Eventing.Messages;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.MediatR.DomainEvents.DomainEventHandler", Version = "1.0")]

namespace Intent.Modules.NET.Tests.Module1.Application.EventHandlers.Customers
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class CustomerCreatedHandler : INotificationHandler<DomainEventNotification<CustomerCreated>>
    {
        private readonly IEventBus _eventBus;

        [IntentManaged(Mode.Merge)]
        public CustomerCreatedHandler(IEventBus eventBus)
        {
            _eventBus = eventBus;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task Handle(
            DomainEventNotification<CustomerCreated> notification,
            CancellationToken cancellationToken)
        {
            var domainEvent = notification.DomainEvent;
            _eventBus.Publish(new CustomerCreatedIEEvent
            {
                Customer = new CustomerCreatedIECustomerDto
                {
                    Id = domainEvent.Customer.Id,
                    Name = domainEvent.Customer.Name
                }
            });
        }
    }
}