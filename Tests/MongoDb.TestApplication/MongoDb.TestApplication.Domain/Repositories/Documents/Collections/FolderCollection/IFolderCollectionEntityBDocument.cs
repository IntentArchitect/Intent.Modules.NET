using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.MongoDb.MongoDbDocumentInterface", Version = "1.0")]

namespace MongoDb.TestApplication.Domain.Repositories.Documents.Collections.FolderCollection
{
    public interface IFolderCollectionEntityBDocument
    {
        string Id { get; }
        string Attribute { get; }
    }
}