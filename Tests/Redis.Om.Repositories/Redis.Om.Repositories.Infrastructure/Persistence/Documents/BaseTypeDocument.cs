using System;
using Intent.RoslynWeaver.Attributes;
using Redis.OM.Modeling;
using Redis.Om.Repositories.Domain.Entities;
using Redis.Om.Repositories.Domain.Repositories.Documents;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Redis.Om.Repositories.Templates.RedisOmDocument", Version = "1.0")]

namespace Redis.Om.Repositories.Infrastructure.Persistence.Documents
{
    [Document(StorageType = StorageType.Json, Prefixes = new[] { "BaseType" })]
    internal class BaseTypeDocument : IBaseTypeDocument, IRedisOmDocument<BaseType, BaseTypeDocument>
    {
        [RedisIdField]
        [Indexed]
        public string Id { get; set; } = default!;
        [Indexed]
        public string BaseName { get; set; } = default!;

        public BaseType ToEntity(BaseType? entity = default)
        {
            entity ??= new BaseType();

            entity.Id = Id ?? throw new Exception($"{nameof(entity.Id)} is null");
            entity.BaseName = BaseName ?? throw new Exception($"{nameof(entity.BaseName)} is null");

            return entity;
        }

        public BaseTypeDocument PopulateFromEntity(BaseType entity)
        {
            Id = entity.Id;
            BaseName = entity.BaseName;

            return this;
        }

        public static BaseTypeDocument? FromEntity(BaseType? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new BaseTypeDocument().PopulateFromEntity(entity);
        }
    }
}