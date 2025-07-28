using Amazon.DynamoDBv2.DataModel;
using DynamoDbTests.EnumAsStrings.Domain.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Aws.DynamoDB.DynamoDBDocument", Version = "1.0")]

namespace DynamoDbTests.EnumAsStrings.Infrastructure.Persistence.Documents
{
    [DynamoDBTable("base-of-ts")]
    internal abstract class BaseOfTDocument<T> : IDynamoDBDocument<BaseOfT<T>, BaseOfTDocument<T>>
    {
        [DynamoDBHashKey]
        public string Id { get; set; } = default!;
        public T GenericAttribute { get; set; } = default!;
        [DynamoDBVersion]
        public int? Version { get; set; }

        public object GetKey() => Id;

        public int? GetVersion() => Version;

        public BaseOfT<T> ToEntity(BaseOfT<T>? entity)
        {
            if (entity is null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            entity.Id = Id ?? throw new Exception($"{nameof(entity.Id)} is null");
            entity.GenericAttribute = GenericAttribute ?? throw new Exception($"{nameof(entity.GenericAttribute)} is null");

            return entity;
        }

        public BaseOfTDocument<T> PopulateFromEntity(BaseOfT<T> entity, Func<object, int?> getVersion)
        {
            Id = entity.Id;
            GenericAttribute = entity.GenericAttribute;
            Version ??= getVersion(GetKey());

            return this;
        }
    }
}