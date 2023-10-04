using System;
using Intent.RoslynWeaver.Attributes;
using Publish.AspNetCore.MassTransit.OutBoxEF.TestApplication.Domain;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.Contracts.IntegrationEventMessage", Version = "1.0")]

namespace MassTransit.Messages.Shared
{
    public record UserDeletedEvent
    {
        public UserDeletedEvent()
        {
            Email = null!;
            UserName = null!;
        }
        public Guid Id { get; init; }
        public string Email { get; init; }
        public string UserName { get; init; }
        public UserType Type { get; init; }
    }
}