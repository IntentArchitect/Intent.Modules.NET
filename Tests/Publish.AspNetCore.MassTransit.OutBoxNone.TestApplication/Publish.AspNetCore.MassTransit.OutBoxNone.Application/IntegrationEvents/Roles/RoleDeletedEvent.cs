using System;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.Contracts.IntegrationEventMessage", Version = "1.0")]

namespace MassTransit.Messages.Shared
{
    public record RoleDeletedEvent
    {
        public Guid Id { get; init; }
        public string Name { get; init; }
    }
}