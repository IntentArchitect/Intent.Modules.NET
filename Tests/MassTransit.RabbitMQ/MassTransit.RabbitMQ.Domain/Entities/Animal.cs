using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;
using MassTransit.RabbitMQ.Domain.Common;
using MassTransit.RabbitMQ.Domain.Events;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace MassTransit.RabbitMQ.Domain.Entities
{
    public class Animal : IHasDomainEvent
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Type { get; set; }

        public List<DomainEvent> DomainEvents { get; set; } = new List<DomainEvent>();

        public void MakeSound()
        {
            DomainEvents.Add(new AnimalSoundRequestedEvent(name: this.Name, type: this.Type));
        }
    }
}