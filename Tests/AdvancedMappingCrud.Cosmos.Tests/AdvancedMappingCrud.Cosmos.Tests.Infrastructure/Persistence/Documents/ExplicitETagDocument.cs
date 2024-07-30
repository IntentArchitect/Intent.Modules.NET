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
    internal class ExplicitETagDocument : IExplicitETagDocument, ICosmosDBDocument<ExplicitETag, ExplicitETagDocument>
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
        [JsonIgnore]
        public string? ETag
        {
            get => _etag;
            set => _etag = value;
        }

        public ExplicitETag ToEntity(ExplicitETag? entity = default)
        {
            entity ??= new ExplicitETag();

            entity.Id = Id ?? throw new Exception($"{nameof(entity.Id)} is null");
            entity.Name = Name ?? throw new Exception($"{nameof(entity.Name)} is null");
            entity.ETag = ETag;

            return entity;
        }

        public ExplicitETagDocument PopulateFromEntity(ExplicitETag entity, Func<string, string?> getEtag)
        {
            Id = entity.Id;
            Name = entity.Name;
            ETag = entity.ETag;

            _etag = _etag == null ? getEtag(((IItem)this).Id) : _etag;

            return this;
        }

        public static ExplicitETagDocument? FromEntity(ExplicitETag? entity, Func<string, string?> getEtag)
        {
            if (entity is null)
            {
                return null;
            }

            return new ExplicitETagDocument().PopulateFromEntity(entity, getEtag);
        }
    }
}