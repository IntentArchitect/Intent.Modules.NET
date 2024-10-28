using System;
using System.Collections.Generic;
using FastEndpointsTest.Domain.Common;
using FastEndpointsTest.Domain.Contracts.DDD;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace FastEndpointsTest.Domain.Entities.DDD
{
    public class DataContractClass : IHasDomainEvent
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public List<DomainEvent> DomainEvents { get; set; } = new List<DomainEvent>();

        public void Change(DataContractObject obj)
        {
            // [IntentFully]
            // TODO: Implement Change (DataContractClass) functionality
            throw new NotImplementedException("Replace with your implementation...");
        }
    }
}