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
    internal class DerivedTypeAggregateDocument : IDerivedTypeAggregateDocument, ICosmosDBDocument<DerivedTypeAggregate, DerivedTypeAggregateDocument>
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

        public DerivedTypeAggregate ToEntity(DerivedTypeAggregate? entity = default)
        {
            entity ??= new DerivedTypeAggregate();

            entity.Id = Id ?? throw new Exception($"{nameof(entity.Id)} is null");

            return entity;
        }

        public DerivedTypeAggregateDocument PopulateFromEntity(DerivedTypeAggregate entity, string? etag = null)
        {
            Id = entity.Id;

            _etag = etag;

            return this;
        }

        public static DerivedTypeAggregateDocument? FromEntity(DerivedTypeAggregate? entity, string? etag = null)
        {
            if (entity is null)
            {
                return null;
            }

            return new DerivedTypeAggregateDocument().PopulateFromEntity(entity, etag);
        }
    }
}