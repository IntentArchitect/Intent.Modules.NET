using Intent.RoslynWeaver.Attributes;
using Mediator;
using TestMediator.NoEventing.Domain.Common;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Mediator.DomainEvents.DomainEventNotification", Version = "1.0")]

namespace TestMediator.NoEventing.Application.Common.Models
{
    public abstract class DomainEventNotification<TDomainEvent> : INotification
        where TDomainEvent : DomainEvent
    {
        protected DomainEventNotification(TDomainEvent domainEvent)
        {
            DomainEvent = domainEvent;
        }

        public TDomainEvent DomainEvent { get; }
    }
}