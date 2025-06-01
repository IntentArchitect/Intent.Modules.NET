using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.DomainEvents.DomainEventBase", Version = "1.0")]

namespace Intent.Modules.NET.Tests.Module1.Domain.Common
{
    public abstract class DomainEvent
    {
        protected DomainEvent()
        {
            DateOccurred = DateTimeOffset.UtcNow;
        }

        public bool IsPublished { get; set; }
        public DateTimeOffset DateOccurred { get; protected set; }
    }
}