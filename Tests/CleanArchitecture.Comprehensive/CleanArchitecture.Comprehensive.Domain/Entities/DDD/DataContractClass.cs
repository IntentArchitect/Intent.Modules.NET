using System;
using System.Collections.Generic;
using CleanArchitecture.Comprehensive.Domain.Common;
using CleanArchitecture.Comprehensive.Domain.Contracts.DDD;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace CleanArchitecture.Comprehensive.Domain.Entities.DDD
{
    public class DataContractClass : IHasDomainEvent
    {
        public DataContractClass()
        {
            Name = null!;
        }

        public Guid Id { get; set; }

        public string Name { get; set; }

        public List<DomainEvent> DomainEvents { get; set; } = [];

        public void Change(DataContractObject obj)
        {

        }
    }
}