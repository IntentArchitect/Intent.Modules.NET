using System;
using System.Collections.Generic;
using CosmosDB.Domain.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace CosmosDB.Domain.Entities.Throughput
{
    public class Autoscale : IHasDomainEvent
    {
        private string? _id;

        public Autoscale()
        {
            Id = null!;
        }

        public string Id
        {
            get => _id ??= Guid.NewGuid().ToString();
            set => _id = value;
        }

        public List<DomainEvent> DomainEvents { get; set; } = [];
    }
}