using System;
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
        [JsonProperty("_etag")]
        protected string? _etag;
        [JsonProperty("type")]
        string IItem.Type
        {
            get => _type ??= GetType().GetNameForDocument();
            set => _type = value;
        }
        string? IItemWithEtag.Etag => _etag;
        public string Id { get; set; } = default!;
        public string Name { get; set; } = default!;
        public List<CountryDocument> Countries { get; set; } = default!;
        IReadOnlyList<ICountryDocument> IRegionDocument.Countries => Countries;

        public Region ToEntity(Region? entity = default)
        {
            entity ??= new Region();

            entity.Id = Id ?? throw new Exception($"{nameof(entity.Id)} is null");
            entity.Name = Name ?? throw new Exception($"{nameof(entity.Name)} is null");
            entity.Countries = Countries.Select(x => x.ToEntity()).ToList();

            return entity;
        }

        public RegionDocument PopulateFromEntity(IRegion entity, Func<string, string?> getEtag)
        {
            Id = entity.Id;
            Name = entity.Name;
            Countries = entity.Countries.Select(x => CountryDocument.FromEntity(x)!).ToList();

            _etag = _etag == null ? getEtag(((IItem)this).Id) : _etag;

            return this;
        }

        public static RegionDocument? FromEntity(IRegion? entity, Func<string, string?> getEtag)
        {
            if (entity is null)
            {
                return null;
            }

            return new RegionDocument().PopulateFromEntity(entity, getEtag);
        }
    }
}