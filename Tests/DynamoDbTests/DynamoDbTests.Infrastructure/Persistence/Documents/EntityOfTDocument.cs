using Amazon.DynamoDBv2.DataModel;
using DynamoDbTests.Domain.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Aws.DynamoDB.DynamoDBDocument", Version = "1.0")]

namespace DynamoDbTests.Infrastructure.Persistence.Documents
{
    [DynamoDBTable("entity-of-ts")]
    internal class EntityOfTDocument<T> : IDynamoDBDocument<EntityOfT<T>, EntityOfTDocument<T>>
    {
        [DynamoDBHashKey]
        public string Id { get; set; } = default!;
        public T GenericAttribute { get; set; } = default!;
        [DynamoDBVersion]
        public int? Version { get; set; }

        public object GetKey() => Id;

        public int? GetVersion() => Version;

        public EntityOfT<T> ToEntity(EntityOfT<T>? entity = default)
        {
            entity ??= new EntityOfT<T>();

            entity.Id = Id ?? throw new Exception($"{nameof(entity.Id)} is null");
            entity.GenericAttribute = GenericAttribute ?? throw new Exception($"{nameof(entity.GenericAttribute)} is null");

            return entity;
        }

        public EntityOfTDocument<T> PopulateFromEntity(EntityOfT<T> entity, Func<object, int?> getVersion)
        {
            Id = entity.Id;
            GenericAttribute = entity.GenericAttribute;
            Version ??= getVersion(GetKey());

            return this;
        }

        public static EntityOfTDocument<T>? FromEntity(EntityOfT<T>? entity, Func<object, int?> getVersion)
        {
            if (entity is null)
            {
                return null;
            }

            return new EntityOfTDocument<T>().PopulateFromEntity(entity, getVersion);
        }
    }
}