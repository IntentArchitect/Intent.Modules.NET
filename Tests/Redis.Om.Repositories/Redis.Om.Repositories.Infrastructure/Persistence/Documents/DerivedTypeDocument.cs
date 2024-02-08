using System;
using Intent.RoslynWeaver.Attributes;
using Redis.OM.Modeling;
using Redis.Om.Repositories.Domain.Entities;
using Redis.Om.Repositories.Domain.Repositories.Documents;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Redis.Om.Repositories.Templates.RedisOmDocument", Version = "1.0")]

namespace Redis.Om.Repositories.Infrastructure.Persistence.Documents
{
    [Document(StorageType = StorageType.Json, Prefixes = new[] { "DerivedType" })]
    internal class DerivedTypeDocument : BaseTypeDocument, IDerivedTypeDocument, IRedisOmDocument<DerivedType, DerivedTypeDocument>
    {
        public DerivedTypeAggregateDocument DerivedTypeAggregate { get; set; } = default!;
        IDerivedTypeAggregateDocument IDerivedTypeDocument.DerivedTypeAggregate => DerivedTypeAggregate;

        public DerivedType ToEntity(DerivedType? entity = default)
        {
            entity ??= new DerivedType();
            entity.DerivedTypeAggregate = DerivedTypeAggregate.ToEntity() ?? throw new Exception($"{nameof(entity.DerivedTypeAggregate)} is null");
            base.ToEntity(entity);

            return entity;
        }

        public DerivedTypeDocument PopulateFromEntity(DerivedType entity)
        {
            DerivedTypeAggregate = DerivedTypeAggregateDocument.FromEntity(entity.DerivedTypeAggregate)!;
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