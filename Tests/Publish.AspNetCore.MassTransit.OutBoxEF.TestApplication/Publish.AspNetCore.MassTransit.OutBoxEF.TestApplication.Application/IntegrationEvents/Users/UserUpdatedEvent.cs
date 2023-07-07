using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;
using Publish.AspNetCore.MassTransit.OutBoxEF.TestApplication.Application.IntegrationEvents;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.Contracts.IntegrationEventMessage", Version = "1.0")]

namespace MassTransit.Messages.Shared
{
    public record UserUpdatedEvent
    {
        public Guid Id { get; init; }
        public string Email { get; init; }
        public string UserName { get; init; }
        public List<PreferenceDto> Preferences { get; init; }
        public UserType Type { get; init; }
    }
}