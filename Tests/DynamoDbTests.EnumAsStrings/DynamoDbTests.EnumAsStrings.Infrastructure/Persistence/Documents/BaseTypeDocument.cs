using Amazon.DynamoDBv2.DataModel;
using DynamoDbTests.EnumAsStrings.Domain.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Aws.DynamoDB.DynamoDBDocument", Version = "1.0")]

namespace DynamoDbTests.EnumAsStrings.Infrastructure.Persistence.Documents
{
    [DynamoDBTable("base-types")]
    internal class BaseTypeDocument : IDynamoDBDocument<BaseType, BaseTypeDocument>
    {
        [DynamoDBHashKey]
        public string Id { get; set; } = default!;
        [DynamoDBVersion]
        public int? Version { get; set; }

        public object GetKey() => Id;

        public int? GetVersion() => Version;

        public BaseType ToEntity(BaseType? entity = default)
        {
            entity ??= new BaseType();

            entity.Id = Id ?? throw new Exception($"{nameof(entity.Id)} is null");

            return entity;
        }

        public BaseTypeDocument PopulateFromEntity(BaseType entity, Func<object, int?> getVersion)
        {
            Id = entity.Id;
            Version ??= getVersion(GetKey());

            return this;
        }

        public static BaseTypeDocument? FromEntity(BaseType? entity, Func<object, int?> getVersion)
        {
            if (entity is null)
            {
                return null;
            }

            return new BaseTypeDocument().PopulateFromEntity(entity, getVersion);
        }
    }
}