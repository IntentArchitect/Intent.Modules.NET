using System;
using AdvancedMappingCrud.Cosmos.Tests.Domain.Entities;
using AdvancedMappingCrud.Cosmos.Tests.Domain.Repositories.Documents;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Azure.CosmosRepository;
using Newtonsoft.Json;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocument", Version = "1.0")]

namespace AdvancedMappingCrud.Cosmos.Tests.Infrastructure.Persistence.Documents
{
    internal class BasicOrderByDocument : IBasicOrderByDocument, ICosmosDBDocument<BasicOrderBy, BasicOrderByDocument>
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
        public string Surname { get; set; } = default!;

        public BasicOrderBy ToEntity(BasicOrderBy? entity = default)
        {
            entity ??= new BasicOrderBy();

            entity.Id = Id ?? throw new Exception($"{nameof(entity.Id)} is null");
            entity.Name = Name ?? throw new Exception($"{nameof(entity.Name)} is null");
            entity.Surname = Surname ?? throw new Exception($"{nameof(entity.Surname)} is null");

            return entity;
        }

        public BasicOrderByDocument PopulateFromEntity(BasicOrderBy entity, Func<string, string?> getEtag)
        {
            Id = entity.Id;
            Name = entity.Name;
            Surname = entity.Surname;

            _etag = _etag == null ? getEtag(((IItem)this).Id) : _etag;

            return this;
        }

        public static BasicOrderByDocument? FromEntity(BasicOrderBy? entity, Func<string, string?> getEtag)
        {
            if (entity is null)
            {
                return null;
            }

            return new BasicOrderByDocument().PopulateFromEntity(entity, getEtag);
        }
    }
}