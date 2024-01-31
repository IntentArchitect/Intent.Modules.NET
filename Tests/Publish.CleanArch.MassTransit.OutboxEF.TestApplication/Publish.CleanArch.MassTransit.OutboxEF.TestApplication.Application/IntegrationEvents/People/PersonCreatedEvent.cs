using System;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.Contracts.IntegrationEventMessage", Version = "1.0")]

namespace Publish.CleanArch.MassTransit.OutboxEF.TestApplication.Services
{
    public record PersonCreatedEvent
    {
        public PersonCreatedEvent()
        {
            FirstName = null!;
            LastName = null!;
        }

        public string FirstName { get; init; }
        public string LastName { get; init; }
        public DateTime DateOfBirth { get; init; }
    }
}