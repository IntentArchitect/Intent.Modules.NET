using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.Contracts.IntegrationEventMessage", Version = "1.0")]

namespace MassTransit.Messages.Shared
{
    public record UserCreatedEvent
    {
        public Guid Id { get; init; }
        public string Email { get; init; }
        public string UserName { get; init; }
        public List<PreferenceDto> Preferences { get; init; }
    }
}