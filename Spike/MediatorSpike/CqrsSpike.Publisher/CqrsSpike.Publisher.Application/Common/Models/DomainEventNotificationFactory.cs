using CqrsSpike.Publisher.Domain.Common;
using CqrsSpike.Publisher.Domain.Events;
using Mediator;

namespace CqrsSpike.Publisher.Application.Common.Models
{
    /// <summary>
    /// Maps a domain event to its Mediator notification without reflection: Mediator's source
    /// generator cannot see open generic notification types such as DomainEventNotification&lt;T&gt;.
    /// Every domain event needs an arm here; an unmapped event throws rather than being dropped.
    /// </summary>
    public static class DomainEventNotificationFactory
    {
        public static INotification Create(DomainEvent domainEvent)
        {
            return domainEvent switch
            {
                OrderPlacedDomainEvent e => new OrderPlacedDomainEventNotification(e),
                _ => throw new InvalidOperationException(
                    $"No notification is mapped for domain event {domainEvent.GetType().Name}. Add one to {nameof(DomainEventNotificationFactory)}.")
            };
        }
    }
}
