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
    internal class DerivedTypeDocument : BaseTypeDocument, IDerivedTypeDocument, ICosmosDBDocument<DerivedType, DerivedTypeDocument>
    {
        [JsonProperty("_etag")]
        private string? etag;
        string? IItemWithEtag.Etag => etag;
        public string DerivedTypeAggregateId { get; set; } = default!;
        public DerivedType ToEntity(DerivedType? entity = default)
        {
            entity ??= new DerivedType();

            entity.DerivedTypeAggregateId = DerivedTypeAggregateId ?? throw new Exception($"{nameof(entity.DerivedTypeAggregateId)} is null");
            base.ToEntity(entity);

            return entity;
        }

        public DerivedTypeDocument PopulateFromEntity(DerivedType entity, string? etag = null)
        {
            DerivedTypeAggregateId = entity.DerivedTypeAggregateId;

            this.etag = etag;
            base.PopulateFromEntity(entity);

            return this;
        }

        public static DerivedTypeDocument? FromEntity(DerivedType? entity, string? etag = null)
        {
            if (entity is null)
            {
                return null;
            }

            return new DerivedTypeDocument().PopulateFromEntity(entity, etag);
        }
    }
}