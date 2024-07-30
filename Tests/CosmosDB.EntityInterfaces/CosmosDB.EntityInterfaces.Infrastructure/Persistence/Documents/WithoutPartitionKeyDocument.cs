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
    internal class WithoutPartitionKeyDocument : IWithoutPartitionKeyDocument, ICosmosDBDocument<IWithoutPartitionKey, WithoutPartitionKey, WithoutPartitionKeyDocument>
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

        public WithoutPartitionKey ToEntity(WithoutPartitionKey? entity = default)
        {
            entity ??= new WithoutPartitionKey();

            entity.Id = Id ?? throw new Exception($"{nameof(entity.Id)} is null");

            return entity;
        }

        public WithoutPartitionKeyDocument PopulateFromEntity(IWithoutPartitionKey entity, Func<string, string?> getEtag)
        {
            Id = entity.Id;

            _etag = _etag == null ? getEtag(((IItem)this).Id) : _etag;

            return this;
        }

        public static WithoutPartitionKeyDocument? FromEntity(IWithoutPartitionKey? entity, Func<string, string?> getEtag)
        {
            if (entity is null)
            {
                return null;
            }

            return new WithoutPartitionKeyDocument().PopulateFromEntity(entity, getEtag);
        }
    }
}