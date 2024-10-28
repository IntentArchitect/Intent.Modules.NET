using System;
using System.Collections.Generic;
using FastEndpointsTest.Domain.Common;
using FastEndpointsTest.Domain.DDD;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.DomainEvents.DomainEvent", Version = "1.0")]

namespace FastEndpointsTest.Domain.Events.DDD
{
    public class AccountTransferStarted : DomainEvent
    {
        public AccountTransferStarted(string toAccNumber, string description, Money amount)
        {
            ToAccNumber = toAccNumber;
            Description = description;
            Amount = amount;
        }

        public string ToAccNumber { get; }
        public string Description { get; }
        public Money Amount { get; }
    }
}