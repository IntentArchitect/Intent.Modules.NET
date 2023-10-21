using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocumentInterface", Version = "1.0")]

namespace CosmosDB.Domain.Repositories.Documents.Folder
{
    public interface IFolderContainerDocument
    {
        string Id { get; }
        string FolderPartitionKey { get; }
    }
}