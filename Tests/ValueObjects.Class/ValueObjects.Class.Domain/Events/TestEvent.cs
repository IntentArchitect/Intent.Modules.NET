using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;
using ValueObjects.Class.Domain.Common;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.DomainEvents.DomainEvent", Version = "1.0")]

namespace ValueObjects.Class.Domain.Events
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