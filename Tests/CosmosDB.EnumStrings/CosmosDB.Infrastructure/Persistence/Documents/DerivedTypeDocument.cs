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
        public string DerivedTypeAggregateId { get; set; } = default!;
        public DerivedType ToEntity(DerivedType? entity = default)
        {
            entity ??= new DerivedType();

            entity.DerivedTypeAggregateId = DerivedTypeAggregateId ?? throw new Exception($"{nameof(entity.DerivedTypeAggregateId)} is null");
            base.ToEntity(entity);

            return entity;
        }

        public DerivedTypeDocument PopulateFromEntity(DerivedType entity, Func<string, string?> getEtag)
        {
            DerivedTypeAggregateId = entity.DerivedTypeAggregateId;

            _etag = _etag == null ? getEtag(((IItem)this).Id) : _etag;
            base.PopulateFromEntity(entity, getEtag);

            return this;
        }

        public static DerivedTypeDocument? FromEntity(DerivedType? entity, Func<string, string?> getEtag)
        {
            if (entity is null)
            {
                return null;
            }

            return new DerivedTypeDocument().PopulateFromEntity(entity, getEtag);
        }
    }
}