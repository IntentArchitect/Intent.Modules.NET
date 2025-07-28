using Amazon.DynamoDBv2.DataModel;
using DynamoDbTests.EnumAsStrings.Domain.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Aws.DynamoDB.DynamoDBDocument", Version = "1.0")]

namespace DynamoDbTests.EnumAsStrings.Infrastructure.Persistence.Documents
{
    [DynamoDBTable("package-containers")]
    internal class PackageContainerDocument : IDynamoDBDocument<PackageContainer, PackageContainerDocument>
    {
        [DynamoDBRangeKey]
        public string Id { get; set; } = default!;
        [DynamoDBHashKey]
        public string PackagePartitionKey { get; set; } = default!;
        [DynamoDBVersion]
        public int? Version { get; set; }

        public object GetKey() => (PackagePartitionKey, Id);

        public int? GetVersion() => Version;

        public PackageContainer ToEntity(PackageContainer? entity = default)
        {
            entity ??= new PackageContainer();

            entity.Id = Id ?? throw new Exception($"{nameof(entity.Id)} is null");
            entity.PackagePartitionKey = PackagePartitionKey ?? throw new Exception($"{nameof(entity.PackagePartitionKey)} is null");

            return entity;
        }

        public PackageContainerDocument PopulateFromEntity(PackageContainer entity, Func<object, int?> getVersion)
        {
            Id = entity.Id;
            PackagePartitionKey = entity.PackagePartitionKey;
            Version ??= getVersion(GetKey());

            return this;
        }

        public static PackageContainerDocument? FromEntity(PackageContainer? entity, Func<object, int?> getVersion)
        {
            if (entity is null)
            {
                return null;
            }

            return new PackageContainerDocument().PopulateFromEntity(entity, getVersion);
        }
    }
}