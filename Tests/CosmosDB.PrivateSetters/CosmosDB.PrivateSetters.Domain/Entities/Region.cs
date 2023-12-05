using System;
using System.Collections.Generic;
using CosmosDB.PrivateSetters.Domain.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace CosmosDB.PrivateSetters.Domain.Entities
{
    public class Region : IHasDomainEvent
    {
        private List<Country> _countries = new List<Country>();
        private string? _id;

        public string Id
        {
            get => _id ??= Guid.NewGuid().ToString();
            private set => _id = value;
        }

        public string Name { get; private set; }

        public IReadOnlyCollection<Country> Countries
        {
            get => _countries.AsReadOnly();
            private set => _countries = new List<Country>(value);
        }

        public List<DomainEvent> DomainEvents { get; set; } = new List<DomainEvent>();
    }
}