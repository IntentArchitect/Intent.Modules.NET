using System;
using System.Collections.Generic;
using CosmosDB.Domain.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace CosmosDB.Domain.Entities
{
    public class Region : IHasDomainEvent
    {
        private string? _id;
        public Region()
        {
            Id = null!;
            Name = null!;
        }
        public string Id
        {
            get => _id ??= Guid.NewGuid().ToString();
            set => _id = value;
        }

        public string Name { get; set; }

        public ICollection<Country> Countries { get; set; } = [];

        public List<DomainEvent> DomainEvents { get; set; } = [];
    }
}