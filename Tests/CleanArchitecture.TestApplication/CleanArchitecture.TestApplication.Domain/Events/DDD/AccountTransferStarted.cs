using System;
using System.Collections.Generic;
using CleanArchitecture.TestApplication.Domain.Common;
using CleanArchitecture.TestApplication.Domain.DDD;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.DomainEvents.DomainEvent", Version = "1.0")]

namespace CleanArchitecture.TestApplication.Domain.Events.DDD
{
    public class AccountTransferStarted : DomainEvent
    {
        public AccountTransferStarted(string toAccNumber, Money amount, string description)
        {
            ToAccNumber = toAccNumber;
            Amount = amount;
            Description = description;
        }

        public string ToAccNumber { get; }

        public Money Amount { get; }

        public string Description { get; }
    }
}