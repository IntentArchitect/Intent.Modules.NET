using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;
using MassTransit.AzureServiceBus.Domain.Common;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.DomainEvents.DomainEvent", Version = "1.0")]

namespace MassTransit.AzureServiceBus.Domain.Events
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