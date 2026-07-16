using Intent.RoslynWeaver.Attributes;
using Wolverine.AspNetCore.Controllers.Domain.Common;
using Wolverine.AspNetCore.Controllers.Domain.Entities;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.DomainEvents.DomainEvent", Version = "1.0")]

namespace Wolverine.AspNetCore.Controllers.Domain.Events
{
    public class OrderPlacedDomainEvent : DomainEvent
    {
        public OrderPlacedDomainEvent(Order order)
        {
            Order = order;
        }

        public Order Order { get; }
    }
}