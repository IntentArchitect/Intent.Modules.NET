using System;
using System.Collections.Generic;
using CosmosDB.EntityInterfaces.Domain.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace CosmosDB.EntityInterfaces.Domain.Entities
{
    public class ClassContainer : IClassContainer, IHasDomainEvent
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