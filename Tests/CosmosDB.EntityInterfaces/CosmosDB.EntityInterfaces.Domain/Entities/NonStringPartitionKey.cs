using System;
using System.Collections.Generic;
using CosmosDB.EntityInterfaces.Domain.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace CosmosDB.EntityInterfaces.Domain.Entities
{
    public class NonStringPartitionKey : INonStringPartitionKey, IHasDomainEvent
    {
        private string? _id;
        private int? _partInt;

        public string Id
        {
            get => _id ??= Guid.NewGuid().ToString();
            set => _id = value;
        }

        public int PartInt
        {
            get => _partInt ?? throw new NullReferenceException("_partInt has not been set");
            set => _partInt = value;
        }

        public string Name { get; set; }

        public List<DomainEvent> DomainEvents { get; set; } = new List<DomainEvent>();
    }
}