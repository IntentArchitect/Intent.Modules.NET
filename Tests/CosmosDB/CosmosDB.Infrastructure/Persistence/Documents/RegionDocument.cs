using System;
using System.Collections.Generic;
using System.Linq;
using CosmosDB.Domain.Common;
using CosmosDB.Domain.Entities;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Azure.CosmosRepository;
using Newtonsoft.Json;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocument", Version = "1.0")]

namespace CosmosDB.Infrastructure.Persistence.Documents
{
    internal class RegionDocument : ICosmosDBDocument<Region, RegionDocument>
    {
        private string? _type;
        public string Id { get; set; } = default!;
        public string Name { get; set; } = default!;
        public ICollection<CountryDocument> Countries { get; set; } = default!;

        public Region ToEntity(Region? entity = default)
        {
            entity ??= new Region();

            entity.Id = Id;
            entity.Name = Name;
            entity.Countries = Countries.Select(x => x.ToEntity()).ToList();

            return entity;
        }
        [JsonProperty("type")]
        string IItem.Type
        {
            get => _type ??= GetType().GetNameForDocument();
            set => _type = value;
        }

        public RegionDocument PopulateFromEntity(Region entity)
        {
            Id = entity.Id;
            Name = entity.Name;
            Countries = entity.Countries.Select(x => CountryDocument.FromEntity(x)!).ToList();

            return this;
        }

        public static RegionDocument? FromEntity(Region? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new RegionDocument().PopulateFromEntity(entity);
        }
    }
}