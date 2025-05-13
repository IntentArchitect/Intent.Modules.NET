using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.DomainEvents.HasDomainEventInterface", Version = "1.0")]

namespace Intent.Modules.NET.Tests.Module2.Domain.Common
{
    public interface IHasDomainEvent
    {
        List<DomainEvent> DomainEvents { get; set; }
    }
}