using DynamoDbTests.PrivateSetters.Domain.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace DynamoDbTests.PrivateSetters.Domain.Entities.Folder
{
    public class FolderContainer : IHasDomainEvent
    {
        private string? _id;
        private string? _folderPartitionKey;
        public FolderContainer()
        {
            Id = null!;
            FolderPartitionKey = null!;
        }

        public string Id
        {
            get => _id ??= Guid.NewGuid().ToString();
            private set => _id = value;
        }

        public string FolderPartitionKey
        {
            get => _folderPartitionKey ??= Guid.NewGuid().ToString();
            private set => _folderPartitionKey = value;
        }

        public List<DomainEvent> DomainEvents { get; set; } = [];
    }
}