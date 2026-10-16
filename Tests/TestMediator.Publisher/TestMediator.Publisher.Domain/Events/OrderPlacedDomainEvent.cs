using Intent.RoslynWeaver.Attributes;
using TestMediator.Publisher.Domain.Common;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.DomainEvents.DomainEvent", Version = "1.0")]

namespace TestMediator.Publisher.Domain.Events
{
    public class OrderPlacedDomainEvent : DomainEvent
    {
        public OrderPlacedDomainEvent()
        {
        }
    }
}