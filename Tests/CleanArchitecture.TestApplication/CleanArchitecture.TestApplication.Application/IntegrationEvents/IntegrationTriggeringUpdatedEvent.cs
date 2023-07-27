using System;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.Contracts.IntegrationEventMessage", Version = "1.0")]

namespace CleanArchitecture.TestApplication.Eventing.Messages
{
    public record IntegrationTriggeringUpdatedEvent
    {
        public Guid Id { get; init; }
    }
}