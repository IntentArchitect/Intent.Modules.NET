using System;
using System.Collections.Generic;
using EntityFrameworkCore.CosmosDb.TestApplication.Domain.Common;
using EntityFrameworkCore.CosmosDb.TestApplication.Domain.ValueObjects;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace EntityFrameworkCore.CosmosDb.TestApplication.Domain.Entities.ValueObjects
{
    public class PersonWithAddressNormal : IHasDomainEvent
    {
        public PersonWithAddressNormal()
        {
            PartitionKey = null!;
            Name = null!;
            AddressNormal = null!;
        }

        public Guid Id { get; set; }

        public string PartitionKey { get; set; }

        public string Name { get; set; }

        public AddressNormal AddressNormal { get; set; }

        public List<DomainEvent> DomainEvents { get; set; } = [];
    }
}