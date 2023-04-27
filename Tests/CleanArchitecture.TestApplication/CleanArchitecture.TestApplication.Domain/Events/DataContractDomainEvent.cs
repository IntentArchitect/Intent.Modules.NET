using System;
using System.Collections.Generic;
using CleanArchitecture.TestApplication.Domain.Common;
using CleanArchitecture.TestApplication.Domain.Contracts.DDD;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.DomainEvents.DomainEvent", Version = "1.0")]

namespace CleanArchitecture.TestApplication.Domain.Events
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