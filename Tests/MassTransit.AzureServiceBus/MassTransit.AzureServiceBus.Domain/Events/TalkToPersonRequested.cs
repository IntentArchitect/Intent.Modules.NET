using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;
using MassTransit.AzureServiceBus.Domain.Common;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.DomainEvents.DomainEvent", Version = "1.0")]

namespace MassTransit.AzureServiceBus.Domain.Events
{
    public class TalkToPersonRequested : DomainEvent
    {
        public TalkToPersonRequested(string message, string firstName, string lastName)
        {
            Message = message;
            FirstName = firstName;
            LastName = lastName;
        }

        public string Message { get; }
        public string FirstName { get; }
        public string LastName { get; }
    }
}