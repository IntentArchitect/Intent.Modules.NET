using System;
using Intent.RoslynWeaver.Attributes;
using Redis.OM.Modeling;
using Redis.Om.Repositories.Domain.Entities;
using Redis.Om.Repositories.Domain.Repositories.Documents;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Redis.Om.Repositories.Templates.RedisOmDocument", Version = "1.0")]

namespace Redis.Om.Repositories.Infrastructure.Persistence.Documents
{
    [Document(StorageType = StorageType.Json, Prefixes = new[] { "EntityOfT" })]
    internal class EntityOfTDocument<T> : IEntityOfTDocument<T>, IRedisOmDocument<EntityOfT<T>, EntityOfTDocument<T>>
    {
        [RedisIdField]
        [Indexed]
        public string Id { get; set; } = default!;
        public T GenericAttribute { get; set; } = default!;

        public EntityOfT<T> ToEntity(EntityOfT<T>? entity = default)
        {
            entity ??= new EntityOfT<T>();

            entity.Id = Id ?? throw new Exception($"{nameof(entity.Id)} is null");
            entity.GenericAttribute = GenericAttribute ?? throw new Exception($"{nameof(entity.GenericAttribute)} is null");

            return entity;
        }

        public EntityOfTDocument<T> PopulateFromEntity(EntityOfT<T> entity)
        {
            Id = entity.Id;
            GenericAttribute = entity.GenericAttribute;

            return this;
        }

        public static EntityOfTDocument<T>? FromEntity(EntityOfT<T>? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new EntityOfTDocument<T>().PopulateFromEntity(entity);
        }
    }
}