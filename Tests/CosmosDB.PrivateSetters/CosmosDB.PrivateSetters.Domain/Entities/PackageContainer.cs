using System;
using System.Collections.Generic;
using CosmosDB.PrivateSetters.Domain.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace CosmosDB.PrivateSetters.Domain.Entities
{
    public class PackageContainer : IHasDomainEvent
    {
        private string? _id;
        public PackageContainer()
        {
            Id = null!;
            PackagePartitionKey = null!;
        }

        public string Id
        {
            get => _id ??= Guid.NewGuid().ToString();
            private set => _id = value;
        }

        public string PackagePartitionKey { get; private set; }

        public List<DomainEvent> DomainEvents { get; set; } = [];
    }
}