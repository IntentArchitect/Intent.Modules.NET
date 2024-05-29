using System;
using System.Collections.Generic;
using CleanArchitecture.Comprehensive.Domain.Common;
using CleanArchitecture.Comprehensive.Domain.DDD;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.DomainEvents.DomainEvent", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Domain.Events.DDD
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