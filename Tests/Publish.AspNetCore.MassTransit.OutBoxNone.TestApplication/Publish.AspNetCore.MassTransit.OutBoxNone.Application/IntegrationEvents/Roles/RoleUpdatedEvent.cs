using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.Contracts.IntegrationEventMessage", Version = "1.0")]

namespace MassTransit.Messages.Shared.Roles
{
    public record RoleUpdatedEvent
    {
        public RoleUpdatedEvent()
        {
            Name = null!;
            Priviledges = null!;
        }

        public Guid Id { get; init; }
        public string Name { get; init; }
        public List<PriviledgeDto> Priviledges { get; init; }
    }
}