using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;
using MassTransit.RabbitMQ.Domain.Common;
using MassTransit.RabbitMQ.Domain.Events;

namespace MassTransit.RabbitMQ.Domain.Entities
{
    public class Animal : IHasDomainEvent
    {
        public Animal()
        {
            Name = null!;
            Type = null!;
        }
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Type { get; set; }

        public List<DomainEvent> DomainEvents { get; set; } = [];

        public void MakeSound()
        {
            DomainEvents.Add(new AnimalSoundRequestedEvent(
                name: this.Name,
                type: this.Type));
        }
    }
}