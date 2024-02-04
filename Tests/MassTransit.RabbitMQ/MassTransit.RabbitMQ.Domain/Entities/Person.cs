using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;
using MassTransit.RabbitMQ.Domain.Common;
using MassTransit.RabbitMQ.Domain.Events;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace MassTransit.RabbitMQ.Domain.Entities
{
    public class Person : IHasDomainEvent
    {
        public Guid Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public List<DomainEvent> DomainEvents { get; set; } = new List<DomainEvent>();

        public void Talk(string message)
        {
            DomainEvents.Add(new TalkToPersonRequested(message: message, firstName: this.FirstName, lastName: this.LastName));
        }
    }
}