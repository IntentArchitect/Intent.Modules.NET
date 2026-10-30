using Intent.RoslynWeaver.Attributes;
using Mediator;
using TestMediator.Publisher.Domain.Common;
using TestMediator.Publisher.Domain.Events;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Mediator.DomainEvents.DomainEventNotificationFactory", Version = "1.0")]

namespace TestMediator.Publisher.Application.Common.Models
{
    /// <summary>
    /// Maps a domain event to its Mediator notification without reflection: Mediator's source
    /// generator cannot see open generic notification types such as DomainEventNotification&lt;T&gt;.
    /// Every domain event has a case here. An unmapped event throws rather than being dropped.
    /// </summary>
    public static class DomainEventNotificationFactory
    {
        public static INotification Create(DomainEvent domainEvent)
        {
            switch (domainEvent)
            {
                case OrderPlacedDomainEvent @event:
                    return new OrderPlacedDomainEventNotification(@event);
                default:
                    throw new InvalidOperationException($"No notification is mapped for domain event {domainEvent.GetType().Name}. Add one to {nameof(DomainEventNotificationFactory)}.");
            }
        }
    }
}