using System;
using CosmosDB.Domain.Entities.Throughput;
using CosmosDB.Domain.Repositories.Documents.Throughput;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Azure.CosmosRepository;
using Newtonsoft.Json;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocument", Version = "1.0")]

namespace CosmosDB.Infrastructure.Persistence.Documents.Throughput
{
    internal class ManualWithValueDocument : IManualWithValueDocument, ICosmosDBDocument<ManualWithValue, ManualWithValueDocument>
    {
        [JsonProperty("_etag")]
        protected string? _etag;
        private string? _type;
        [JsonProperty("type")]
        string IItem.Type
        {
            get => _type ??= GetType().GetNameForDocument();
            set => _type = value;
        }
        string? IItemWithEtag.Etag => _etag;
        public string Id { get; set; } = default!;

        public ManualWithValue ToEntity(ManualWithValue? entity = default)
        {
            entity ??= new ManualWithValue();

            entity.Id = Id ?? throw new Exception($"{nameof(entity.Id)} is null");

            return entity;
        }

        public ManualWithValueDocument PopulateFromEntity(ManualWithValue entity, Func<string, string?> getEtag)
        {
            Id = entity.Id;

            _etag = _etag == null ? getEtag(((IItem)this).Id) : _etag;

            return this;
        }

        public static ManualWithValueDocument? FromEntity(ManualWithValue? entity, Func<string, string?> getEtag)
        {
            if (entity is null)
            {
                return null;
            }

            return new ManualWithValueDocument().PopulateFromEntity(entity, getEtag);
        }
    }
}