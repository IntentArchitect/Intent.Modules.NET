using System.Collections.Generic;
using System.Linq;
using CosmosDB.EntityInterfaces.Domain.Entities;
using CosmosDB.EntityInterfaces.Domain.Repositories.Documents;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Azure.CosmosRepository;
using Newtonsoft.Json;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocument", Version = "1.0")]

namespace CosmosDB.EntityInterfaces.Infrastructure.Persistence.Documents
{
    internal class RegionDocument : IRegionDocument, ICosmosDBDocument<IRegion, Region, RegionDocument>
    {
        private string? _type;
        [JsonProperty("type")]
        string IItem.Type
        {
            get => _type ??= GetType().GetNameForDocument();
            set => _type = value;
        }
        public string Id { get; set; } = default!;
        public string Name { get; set; } = default!;
        public List<CountryDocument> Countries { get; set; } = default!;
        IReadOnlyList<ICountryDocument> IRegionDocument.Countries => Countries;

        public Region ToEntity(Region? entity = default)
        {
            entity ??= new Region();

            entity.Id = Id;
            entity.Name = Name;
            entity.Countries = Countries.Select(x => x.ToEntity()).ToList();

            return entity;
        }

        public RegionDocument PopulateFromEntity(IRegion entity)
        {
            Id = entity.Id;
            Name = entity.Name;
            Countries = entity.Countries.Select(x => CountryDocument.FromEntity(x)!).ToList();

            return this;
        }

        public static RegionDocument? FromEntity(IRegion? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new RegionDocument().PopulateFromEntity(entity);
        }
    }
}