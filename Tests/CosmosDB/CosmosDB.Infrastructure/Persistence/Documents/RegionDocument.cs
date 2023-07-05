using System;
using CosmosDB.Domain.Entities;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Azure.CosmosRepository;
using Newtonsoft.Json;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocument", Version = "1.0")]

namespace CosmosDB.Infrastructure.Persistence.Documents
{
    internal class RegionDocument : Region, ICosmosDBDocument<RegionDocument, Region>
    {
        private string? _type;
        [JsonProperty("id")]
        public new string Id
        {
            get => base.Id ??= Guid.NewGuid().ToString();
            set => base.Id = value;
        }
        [JsonProperty("type")]
        string IItem.Type
        {
            get => _type ??= GetType().Name;
            set => _type = value;
        }

        public RegionDocument PopulateFromEntity(Region entity)
        {
            Id = entity.Id;
            Name = entity.Name;
            Countries = entity.Countries;

            return this;
        }
    }
}