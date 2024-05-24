using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;
using ValueObjects.Record.Domain.Common;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.DomainEvents.DomainEvent", Version = "1.0")]

namespace ValueObjects.Record.Domain.Events
{
    public class TestEvent : DomainEvent
    {
        public TestEvent(Address address, Money amount)
        {
            Address = address;
            Amount = amount;
        }

        public Address Address { get; }
        public Money Amount { get; }
    }
}