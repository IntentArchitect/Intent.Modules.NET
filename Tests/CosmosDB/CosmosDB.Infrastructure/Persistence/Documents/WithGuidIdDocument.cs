using System;
using CosmosDB.Domain.Entities;
using CosmosDB.Domain.Repositories.Documents;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Azure.CosmosRepository;
using Newtonsoft.Json;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocument", Version = "1.0")]

namespace CosmosDB.Infrastructure.Persistence.Documents
{
    internal class WithGuidIdDocument : IWithGuidIdDocument, ICosmosDBDocument<WithGuidId, WithGuidIdDocument>
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
        public string Id { get; set; }
        public string Field { get; set; } = default!;

        public WithGuidId ToEntity(WithGuidId? entity = default)
        {
            entity ??= new WithGuidId();

            entity.Id = Guid.Parse(Id);
            entity.Field = Field ?? throw new Exception($"{nameof(entity.Field)} is null");

            return entity;
        }

        public WithGuidIdDocument PopulateFromEntity(WithGuidId entity, Func<string, string?> getEtag)
        {
            Id = entity.Id.ToString();
            Field = entity.Field;

            _etag = _etag == null ? getEtag(((IItem)this).Id) : _etag;

            return this;
        }

        public static WithGuidIdDocument? FromEntity(WithGuidId? entity, Func<string, string?> getEtag)
        {
            if (entity is null)
            {
                return null;
            }

            return new WithGuidIdDocument().PopulateFromEntity(entity, getEtag);
        }
    }
}