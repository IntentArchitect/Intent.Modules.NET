using Intent.RoslynWeaver.Attributes;
using Mediator;
using TestMediator.NoEventing.Application.Common.Models.Products;
using TestMediator.NoEventing.Domain.Common;
using TestMediator.NoEventing.Domain.Events.Products;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Mediator.DomainEvents.DomainEventNotificationFactory", Version = "1.0")]

namespace TestMediator.NoEventing.Application.Common.Models
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
                case ProductPriceChangedDomainEvent @event:
                    return new ProductPriceChangedDomainEventNotification(@event);
                default:
                    throw new InvalidOperationException($"No notification is mapped for domain event {domainEvent.GetType().Name}. Add one to {nameof(DomainEventNotificationFactory)}.");
            }
        }
    }
}