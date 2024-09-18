using System;
using System.Collections.Generic;
using CosmosDB.Domain.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace CosmosDB.Domain.Entities
{
    public class ClassContainer : IHasDomainEvent
    {
        private string? _id;
        private string? _classPartitionKey;
        public string Id
        {
            get => _id ??= Guid.NewGuid().ToString();
            set => _id = value;
        }

        public string ClassPartitionKey
        {
            get => _classPartitionKey ??= Guid.NewGuid().ToString();
            set => _classPartitionKey = value;
        }

        public List<DomainEvent> DomainEvents { get; set; } = new List<DomainEvent>();
    }
}