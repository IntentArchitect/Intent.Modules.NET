using Amazon.DynamoDBv2.DataModel;
using DynamoDbTests.EntityInterfaces.Domain.Entities.Folder;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Aws.DynamoDB.DynamoDBDocument", Version = "1.0")]

namespace DynamoDbTests.EntityInterfaces.Infrastructure.Persistence.Documents.Folder
{
    [DynamoDBTable("folder-containers")]
    internal class FolderContainerDocument : IDynamoDBDocument<IFolderContainer, FolderContainer, FolderContainerDocument>
    {
        [DynamoDBRangeKey]
        public string Id { get; set; } = default!;
        [DynamoDBHashKey]
        public string FolderPartitionKey { get; set; } = default!;
        [DynamoDBVersion]
        public int? Version { get; set; }

        public object GetKey() => (FolderPartitionKey, Id);

        public int? GetVersion() => Version;

        public FolderContainer ToEntity(FolderContainer? entity = default)
        {
            entity ??= new FolderContainer();

            entity.Id = Id ?? throw new Exception($"{nameof(entity.Id)} is null");
            entity.FolderPartitionKey = FolderPartitionKey ?? throw new Exception($"{nameof(entity.FolderPartitionKey)} is null");

            return entity;
        }

        public FolderContainerDocument PopulateFromEntity(IFolderContainer entity, Func<object, int?> getVersion)
        {
            Id = entity.Id;
            FolderPartitionKey = entity.FolderPartitionKey;
            Version ??= getVersion(GetKey());

            return this;
        }

        public static FolderContainerDocument? FromEntity(IFolderContainer? entity, Func<object, int?> getVersion)
        {
            if (entity is null)
            {
                return null;
            }

            return new FolderContainerDocument().PopulateFromEntity(entity, getVersion);
        }
    }
}