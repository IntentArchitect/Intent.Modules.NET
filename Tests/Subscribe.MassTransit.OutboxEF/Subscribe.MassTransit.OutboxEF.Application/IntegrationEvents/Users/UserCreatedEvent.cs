using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;
using Subscribe.MassTransit.OutboxEF.Application.IntegrationEvents;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.Contracts.IntegrationEventMessage", Version = "1.0")]

namespace MassTransit.Messages.Shared.Users
{
    public record UserCreatedEvent
    {
        public UserCreatedEvent()
        {
            Email = null!;
            UserName = null!;
            Preferences = null!;
        }

        public Guid Id { get; init; }
        public string Email { get; init; }
        public string UserName { get; init; }
        public UserType Type { get; init; }
        public List<PreferenceDto> Preferences { get; init; }
    }
}