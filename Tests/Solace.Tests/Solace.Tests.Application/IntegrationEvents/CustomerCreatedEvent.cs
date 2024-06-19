using System;
using Intent.RoslynWeaver.Attributes;
using Solace.Tests.Application;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.Contracts.IntegrationEventMessage", Version = "1.0")]

namespace Solace.Tests.Eventing.Messages
{
    public record CustomerCreatedEvent : BaseMessage
    {
        public CustomerCreatedEvent()
        {
            Name = null!;
            Surname = null!;
        }

        public Guid Id { get; init; }
        public string Name { get; init; }
        public string Surname { get; init; }
    }
}