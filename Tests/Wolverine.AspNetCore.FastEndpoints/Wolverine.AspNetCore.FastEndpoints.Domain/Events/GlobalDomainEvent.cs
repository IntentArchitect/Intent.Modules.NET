using Intent.RoslynWeaver.Attributes;
using Wolverine.AspNetCore.FastEndpoints.Domain.Common;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.DomainEvents.DomainEvent", Version = "1.0")]

namespace Wolverine.AspNetCore.FastEndpoints.Domain.Events
{
    public class GlobalDomainEvent : DomainEvent
    {
        public GlobalDomainEvent()
        {
        }
    }
}