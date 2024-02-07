using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;
using MassTransit.RabbitMQ.Domain.Common;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.DomainEvents.DomainEvent", Version = "1.0")]

namespace MassTransit.RabbitMQ.Domain.Events
{
    public class AnimalSoundRequestedEvent : DomainEvent
    {
        public AnimalSoundRequestedEvent(string name, string type)
        {
            Name = name;
            Type = type;
        }

        public string Name { get; }
        public string Type { get; }
    }
}