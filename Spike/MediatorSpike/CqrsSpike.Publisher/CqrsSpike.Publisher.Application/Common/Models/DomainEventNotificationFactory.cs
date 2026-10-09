using CqrsSpike.Publisher.Domain.Common;
using CqrsSpike.Publisher.Domain.Events;
using Mediator;

namespace CqrsSpike.Publisher.Application.Common.Models
{
    /// <summary>
    /// Maps a domain event to its Mediator notification. Generated per modelled domain event so that
    /// dispatch stays free of reflection: Mediator's source generator cannot see open generic
    /// notification types such as DomainEventNotification&lt;T&gt;.
    /// </summary>
    public static class DomainEventNotificationFactory
    {
        public static INotification? Create(DomainEvent domainEvent)
        {
            return domainEvent switch
            {
                OrderPlacedDomainEvent e => new OrderPlacedDomainEventNotification(e),
                _ => null
            };
        }
    }
}
