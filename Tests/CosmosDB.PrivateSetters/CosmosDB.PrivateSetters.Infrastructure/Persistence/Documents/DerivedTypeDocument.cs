using System;
using CosmosDB.PrivateSetters.Domain.Entities;
using CosmosDB.PrivateSetters.Domain.Repositories.Documents;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocument", Version = "1.0")]

namespace CosmosDB.PrivateSetters.Infrastructure.Persistence.Documents
{
    internal class DerivedTypeDocument : BaseTypeDocument, IDerivedTypeDocument, ICosmosDBDocument<DerivedType, DerivedTypeDocument>
    {
        public string DerivedTypeAggregateId { get; set; } = default!;

        public DerivedType ToEntity(DerivedType? entity = default)
        {
            entity ??= new DerivedType();

            ReflectionHelper.ForceSetProperty(entity, nameof(DerivedTypeAggregateId), DerivedTypeAggregateId ?? throw new Exception($"{nameof(entity.DerivedTypeAggregateId)} is null"));
            base.ToEntity(entity);

            return entity;
        }

        public DerivedTypeDocument PopulateFromEntity(DerivedType entity)
        {
            DerivedTypeAggregateId = entity.DerivedTypeAggregateId;
            base.PopulateFromEntity(entity);

            return this;
        }

        public static DerivedTypeDocument? FromEntity(DerivedType? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new DerivedTypeDocument().PopulateFromEntity(entity);
        }
    }
}