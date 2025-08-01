using System;
using System.Collections.Generic;
using EntityFrameworkCore.CosmosDb.TestApplication.Domain.Common;
using EntityFrameworkCore.CosmosDb.TestApplication.Domain.ValueObjects;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace EntityFrameworkCore.CosmosDb.TestApplication.Domain.Entities.ValueObjects
{
    public class PersonWithAddressSerialized : IHasDomainEvent
    {
        public PersonWithAddressSerialized()
        {
            PartitionKey = null!;
            Name = null!;
            AddressSerialized = null!;
        }

        public Guid Id { get; set; }

        public string PartitionKey { get; set; }

        public string Name { get; set; }

        public AddressSerialized AddressSerialized { get; set; }

        public List<DomainEvent> DomainEvents { get; set; } = [];
    }
}