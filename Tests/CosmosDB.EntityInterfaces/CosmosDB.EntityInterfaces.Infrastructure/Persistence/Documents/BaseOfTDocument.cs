using System;
using CosmosDB.EntityInterfaces.Domain.Entities;
using CosmosDB.EntityInterfaces.Domain.Repositories.Documents;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Azure.CosmosRepository;
using Newtonsoft.Json;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocument", Version = "1.0")]

namespace CosmosDB.EntityInterfaces.Infrastructure.Persistence.Documents
{
    internal abstract class BaseOfTDocument<T> : IBaseOfTDocument<T>, ICosmosDBDocument<IBaseOfT<T>, BaseOfT<T>, BaseOfTDocument<T>>
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

        public BaseOfT<T> ToEntity(BaseOfT<T>? entity)
        {
            if (entity is null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            entity.Id = Id ?? throw new Exception($"{nameof(entity.Id)} is null");
            entity.GenericAttribute = GenericAttribute ?? throw new Exception($"{nameof(entity.GenericAttribute)} is null");

            return entity;
        }

        public BaseOfTDocument<T> PopulateFromEntity(IBaseOfT<T> entity, Func<string, string?> getEtag)
        {
            Id = entity.Id;
            GenericAttribute = entity.GenericAttribute;

            _etag = _etag == null ? getEtag(((IItem)this).Id) : _etag;

            return this;
        }
    }
}