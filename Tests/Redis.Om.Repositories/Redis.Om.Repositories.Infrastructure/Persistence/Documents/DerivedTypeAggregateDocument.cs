using System;
using Intent.RoslynWeaver.Attributes;
using Redis.OM.Modeling;
using Redis.Om.Repositories.Domain.Entities;
using Redis.Om.Repositories.Domain.Repositories.Documents;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Redis.Om.Repositories.Templates.RedisOmDocument", Version = "1.0")]

namespace Redis.Om.Repositories.Infrastructure.Persistence.Documents
{
    internal class DerivedTypeAggregateDocument : IDerivedTypeAggregateDocument
    {
        [Indexed]
        public string AggregateName { get; set; } = default!;
        public DerivedTypeAggregate ToEntity(DerivedTypeAggregate? entity = default)
        {
            entity ??= new DerivedTypeAggregate();

            entity.AggregateName = AggregateName ?? throw new Exception($"{nameof(entity.AggregateName)} is null");

            return entity;
        }

        public DerivedTypeAggregateDocument PopulateFromEntity(DerivedTypeAggregate entity)
        {
            AggregateName = entity.AggregateName;

            return this;
        }

        public static DerivedTypeAggregateDocument? FromEntity(DerivedTypeAggregate? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new DerivedTypeAggregateDocument().PopulateFromEntity(entity);
        }
    }
}