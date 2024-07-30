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
    internal class EntityOfTDocument<T> : IEntityOfTDocument<T>, ICosmosDBDocument<EntityOfT<T>, EntityOfTDocument<T>>
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
        public T GenericAttribute { get; set; } = default!;

        public EntityOfT<T> ToEntity(EntityOfT<T>? entity = default)
        {
            entity ??= new EntityOfT<T>();

            entity.Id = Id ?? throw new Exception($"{nameof(entity.Id)} is null");
            entity.GenericAttribute = GenericAttribute ?? throw new Exception($"{nameof(entity.GenericAttribute)} is null");

            return entity;
        }

        public EntityOfTDocument<T> PopulateFromEntity(EntityOfT<T> entity, Func<string, string?> getEtag)
        {
            Id = entity.Id;
            GenericAttribute = entity.GenericAttribute;

            _etag = _etag == null ? getEtag(((IItem)this).Id) : _etag;

            return this;
        }

        public static EntityOfTDocument<T>? FromEntity(EntityOfT<T>? entity, Func<string, string?> getEtag)
        {
            if (entity is null)
            {
                return null;
            }

            return new EntityOfTDocument<T>().PopulateFromEntity(entity, getEtag);
        }
    }
}