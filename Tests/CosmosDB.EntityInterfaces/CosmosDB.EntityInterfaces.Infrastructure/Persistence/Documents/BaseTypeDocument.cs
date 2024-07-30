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
    internal class BaseTypeDocument : IBaseTypeDocument, ICosmosDBDocument<IBaseType, BaseType, BaseTypeDocument>
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

        public BaseType ToEntity(BaseType? entity = default)
        {
            entity ??= new BaseType();

            entity.Id = Id ?? throw new Exception($"{nameof(entity.Id)} is null");

            return entity;
        }

        public BaseTypeDocument PopulateFromEntity(IBaseType entity, Func<string, string?> getEtag)
        {
            Id = entity.Id;

            _etag = _etag == null ? getEtag(((IItem)this).Id) : _etag;

            return this;
        }

        public static BaseTypeDocument? FromEntity(IBaseType? entity, Func<string, string?> getEtag)
        {
            if (entity is null)
            {
                return null;
            }

            return new BaseTypeDocument().PopulateFromEntity(entity, getEtag);
        }
    }
}