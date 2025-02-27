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
    internal class StockDocumentDocument : IStockDocumentDocument, ICosmosDBDocument<Domain.Entities.StockDocument, StockDocumentDocument>
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

        public Domain.Entities.StockDocument ToEntity(Domain.Entities.StockDocument? entity = default)
        {
            entity ??= new Domain.Entities.StockDocument();

            entity.Id = Id ?? throw new Exception($"{nameof(entity.Id)} is null");

            return entity;
        }

        public StockDocumentDocument PopulateFromEntity(
            Domain.Entities.StockDocument entity,
            Func<string, string?> getEtag)
        {
            Id = entity.Id;

            _etag = _etag == null ? getEtag(((IItem)this).Id) : _etag;

            return this;
        }

        public static StockDocumentDocument? FromEntity(
            Domain.Entities.StockDocument? entity,
            Func<string, string?> getEtag)
        {
            if (entity is null)
            {
                return null;
            }

            return new StockDocumentDocument().PopulateFromEntity(entity, getEtag);
        }
    }
}