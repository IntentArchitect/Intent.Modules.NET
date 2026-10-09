using CqrsSpike.Publisher.Domain.Common;
using Intent.RoslynWeaver.Attributes;
using Mediator;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Mediator.DomainEvents.DomainEventNotification", Version = "1.0")]

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