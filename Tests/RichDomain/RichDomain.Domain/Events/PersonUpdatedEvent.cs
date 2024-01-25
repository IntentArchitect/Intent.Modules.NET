using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;
using RichDomain.Domain.Common;
using RichDomain.Domain.Entities;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.DomainEvents.DomainEvent", Version = "1.0")]

namespace RichDomain.Domain.Events
{
    public class PersonUpdatedEvent : DomainEvent
    {
        public PersonUpdatedEvent(Person person)
        {
            Person = person;
        }

        public Person Person { get; }
    }
}