using Amazon.DynamoDBv2.DataModel;
using DynamoDbTests.Domain.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Aws.DynamoDB.DynamoDBDocument", Version = "1.0")]

namespace DynamoDbTests.Infrastructure.Persistence.Documents
{
    [DynamoDBTable("classes")]
    internal class ClassContainerDocument : IDynamoDBDocument<ClassContainer, ClassContainerDocument>
    {
        [DynamoDBRangeKey]
        public string Id { get; set; } = default!;
        [DynamoDBHashKey]
        public string ClassPartitionKey { get; set; } = default!;
        [DynamoDBVersion]
        public int? Version { get; set; }

        public object GetKey() => (ClassPartitionKey, Id);

        public int? GetVersion() => Version;

        public ClassContainer ToEntity(ClassContainer? entity = default)
        {
            entity ??= new ClassContainer();

            entity.Id = Id ?? throw new Exception($"{nameof(entity.Id)} is null");
            entity.ClassPartitionKey = ClassPartitionKey ?? throw new Exception($"{nameof(entity.ClassPartitionKey)} is null");

            return entity;
        }

        public ClassContainerDocument PopulateFromEntity(ClassContainer entity, Func<object, int?> getVersion)
        {
            Id = entity.Id;
            ClassPartitionKey = entity.ClassPartitionKey;
            Version ??= getVersion(GetKey());

            return this;
        }

        public static ClassContainerDocument? FromEntity(ClassContainer? entity, Func<object, int?> getVersion)
        {
            if (entity is null)
            {
                return null;
            }

            return new ClassContainerDocument().PopulateFromEntity(entity, getVersion);
        }
    }
}