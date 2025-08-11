using Amazon.DynamoDBv2.DataModel;
using DynamoDbTests.Domain.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Aws.DynamoDB.DynamoDBDocument", Version = "1.0")]

namespace DynamoDbTests.Infrastructure.Persistence.Documents
{
    [DynamoDBTable("non-string-partition-keys")]
    internal class NonStringPartitionKeyDocument : IDynamoDBDocument<NonStringPartitionKey, NonStringPartitionKeyDocument>
    {
        [DynamoDBRangeKey]
        public string Id { get; set; } = default!;
        [DynamoDBHashKey]
        public int PartInt { get; set; }
        public string Name { get; set; } = default!;
        [DynamoDBVersion]
        public int? Version { get; set; }

        public object GetKey() => (PartInt, Id);

        public int? GetVersion() => Version;

        public NonStringPartitionKey ToEntity(NonStringPartitionKey? entity = default)
        {
            entity ??= new NonStringPartitionKey();

            entity.Id = Id ?? throw new Exception($"{nameof(entity.Id)} is null");
            entity.PartInt = PartInt;
            entity.Name = Name ?? throw new Exception($"{nameof(entity.Name)} is null");

            return entity;
        }

        public NonStringPartitionKeyDocument PopulateFromEntity(
            NonStringPartitionKey entity,
            Func<object, int?> getVersion)
        {
            Id = entity.Id;
            PartInt = entity.PartInt;
            Name = entity.Name;
            Version ??= getVersion(GetKey());

            return this;
        }

        public static NonStringPartitionKeyDocument? FromEntity(
            NonStringPartitionKey? entity,
            Func<object, int?> getVersion)
        {
            if (entity is null)
            {
                return null;
            }

            return new NonStringPartitionKeyDocument().PopulateFromEntity(entity, getVersion);
        }
    }
}