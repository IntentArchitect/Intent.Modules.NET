using System;
using System.Collections.Generic;
using System.Linq;
using CosmosDB.EntityInterfaces.Domain.Common;
using CosmosDB.EntityInterfaces.Domain.Entities.Common;
using Intent.RoslynWeaver.Attributes;

namespace CosmosDB.EntityInterfaces.Domain.Entities
{
    public class Region : IRegion, IHasDomainEvent
    {
        private string? _id;

        public string Id
        {
            get => _id ??= Guid.NewGuid().ToString();
            set => _id = value;
        }

        public string Name { get; set; }

        public ICollection<Country> Countries { get; set; } = new List<Country>();

        ICollection<ICountry> IRegion.Countries
        {
            get => Countries.CreateWrapper<ICountry, Country>();
            set => Countries = value.Cast<Country>().ToList();
        }

        public List<DomainEvent> DomainEvents { get; set; } = new List<DomainEvent>();
    }
}