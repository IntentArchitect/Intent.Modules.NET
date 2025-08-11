using DynamoDbTests.EntityInterfaces.Domain.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntityInterface", Version = "1.0")]

namespace DynamoDbTests.EntityInterfaces.Domain.Entities.Folder
{
    public interface IFolderContainer : IHasDomainEvent
    {
        string Id { get; set; }

        string FolderPartitionKey { get; set; }
    }
}