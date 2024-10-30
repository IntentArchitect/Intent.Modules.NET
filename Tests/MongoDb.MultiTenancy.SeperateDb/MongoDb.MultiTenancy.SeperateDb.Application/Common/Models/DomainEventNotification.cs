using Intent.RoslynWeaver.Attributes;
using MediatR;
using MongoDb.MultiTenancy.SeperateDb.Domain.Common;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.MediatR.DomainEvents.DomainEventNotification", Version = "1.0")]

namespace MongoDb.MultiTenancy.SeperateDb.Application.Common.Models
{
    public class DomainEventNotification<TDomainEvent> : INotification where TDomainEvent : DomainEvent
    {
        public DomainEventNotification(TDomainEvent domainEvent)
        {
            DomainEvent = domainEvent;
        }

        public TDomainEvent DomainEvent { get; }
    }
}