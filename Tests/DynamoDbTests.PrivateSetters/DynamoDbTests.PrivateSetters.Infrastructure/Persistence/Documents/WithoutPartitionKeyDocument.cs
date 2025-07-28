using Amazon.DynamoDBv2.DataModel;
using DynamoDbTests.PrivateSetters.Domain.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Aws.DynamoDB.DynamoDBDocument", Version = "1.0")]

namespace DynamoDbTests.PrivateSetters.Infrastructure.Persistence.Documents
{
    [DynamoDBTable("without-partition-keys")]
    internal class WithoutPartitionKeyDocument : IDynamoDBDocument<WithoutPartitionKey, WithoutPartitionKeyDocument>
    {
        [DynamoDBHashKey]
        public string Id { get; set; } = default!;
        [DynamoDBVersion]
        public int? Version { get; set; }

        public object GetKey() => Id;

        public int? GetVersion() => Version;

        public WithoutPartitionKey ToEntity(WithoutPartitionKey? entity = default)
        {
            entity ??= new WithoutPartitionKey();

            ReflectionHelper.ForceSetProperty(entity, nameof(Id), Id ?? throw new Exception($"{nameof(entity.Id)} is null"));

            return entity;
        }

        public WithoutPartitionKeyDocument PopulateFromEntity(WithoutPartitionKey entity, Func<object, int?> getVersion)
        {
            Id = entity.Id;
            Version ??= getVersion(GetKey());

            return this;
        }

        public static WithoutPartitionKeyDocument? FromEntity(WithoutPartitionKey? entity, Func<object, int?> getVersion)
        {
            if (entity is null)
            {
                return null;
            }

            return new WithoutPartitionKeyDocument().PopulateFromEntity(entity, getVersion);
        }
    }
}