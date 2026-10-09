using CqrsSpike.Publisher.Domain.Common;
using Mediator;

namespace CqrsSpike.Publisher.Application.Common.Models
{
    public abstract class DomainEventNotification<TDomainEvent> : INotification where TDomainEvent : DomainEvent
    {
        protected DomainEventNotification(TDomainEvent domainEvent)
        {
            DomainEvent = domainEvent;
        }

        public TDomainEvent DomainEvent { get; }
    }
}