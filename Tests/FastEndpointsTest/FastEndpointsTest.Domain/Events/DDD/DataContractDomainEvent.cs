using System;
using System.Collections.Generic;
using FastEndpointsTest.Domain.Common;
using FastEndpointsTest.Domain.Contracts.DDD;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.DomainEvents.DomainEvent", Version = "1.0")]

namespace FastEndpointsTest.Domain.Events.DDD
{
    public class DataContractDomainEvent : DomainEvent
    {
        public DataContractDomainEvent(DataContractObject contract)
        {
            Contract = contract;
        }

        public DataContractObject Contract { get; }
    }
}