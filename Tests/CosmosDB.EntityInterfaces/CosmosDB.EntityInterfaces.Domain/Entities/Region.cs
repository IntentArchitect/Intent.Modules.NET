using System;
using System.Collections.Generic;
using System.Linq;
using CosmosDB.EntityInterfaces.Domain.Common;
using CosmosDB.EntityInterfaces.Domain.Entities.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace CosmosDB.EntityInterfaces.Domain.Entities
{
    public class Region : IRegion, IHasDomainEvent
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

        ICollection<ICountry> IRegion.Countries
        {
            get => Countries.CreateWrapper<ICountry, Country>();
            set => Countries = value.Cast<Country>().ToList();
        }

        public List<DomainEvent> DomainEvents { get; set; } = [];
    }
}