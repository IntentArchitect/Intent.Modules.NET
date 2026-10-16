using Intent.RoslynWeaver.Attributes;
using TestMediator.Publisher.Domain.Events;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Mediator.DomainEvents.TypedDomainEventNotification", Version = "1.0")]

namespace TestMediator.Publisher.Application.Common.Models
{
    public sealed class OrderPlacedDomainEventNotification : DomainEventNotification<OrderPlacedDomainEvent>
    {
        public OrderPlacedDomainEventNotification(OrderPlacedDomainEvent domainEvent) : base(domainEvent)
        {
        }
    }
}