using System;
using System.Collections.Generic;
using CosmosDB.PrivateSetters.Domain.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace CosmosDB.PrivateSetters.Domain.Entities.Throughput
{
    public class AutoscaleWithMaxValue : IHasDomainEvent
    {
        private string? _id;

        public AutoscaleWithMaxValue()
        {
            Id = null!;
        }

        public string Id
        {
            get => _id ??= Guid.NewGuid().ToString();
            private set => _id = value;
        }

        public List<DomainEvent> DomainEvents { get; set; } = [];
    }
}