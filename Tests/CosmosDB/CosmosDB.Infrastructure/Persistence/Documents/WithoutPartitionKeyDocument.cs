using System;
using System.Collections.Generic;
using CosmosDB.Domain.Common;
using CosmosDB.Domain.Entities;
using CosmosDB.Domain.Repositories.Documents;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Azure.CosmosRepository;
using Newtonsoft.Json;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocument", Version = "1.0")]

namespace CosmosDB.Infrastructure.Persistence.Documents
{
    internal class WithoutPartitionKeyDocument : IWithoutPartitionKeyDocument, ICosmosDBDocument<WithoutPartitionKey, WithoutPartitionKeyDocument>
    {
        private string? _type;
        [JsonProperty("_etag")]
        private string? etag;
        public string Id { get; set; } = default!;

        public WithoutPartitionKey ToEntity(WithoutPartitionKey? entity = default)
        {
            entity ??= new WithoutPartitionKey();

            entity.Id = Id ?? throw new Exception($"{nameof(entity.Id)} is null");

            return entity;
        }
        [JsonProperty("type")]
        string IItem.Type
        {
            get => _type ??= GetType().GetNameForDocument();
            set => _type = value;
        }
        string? IItemWithEtag.Etag => etag;

        public WithoutPartitionKeyDocument PopulateFromEntity(WithoutPartitionKey entity, string? etag = null)
        {
            Id = entity.Id;

            this.etag = etag;

            return this;
        }

        public static WithoutPartitionKeyDocument? FromEntity(WithoutPartitionKey? entity, string? etag = null)
        {
            if (entity is null)
            {
                return null;
            }

            return new WithoutPartitionKeyDocument().PopulateFromEntity(entity, etag);
        }
    }
}