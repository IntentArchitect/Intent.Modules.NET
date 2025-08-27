using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.MongoDb.MongoDbDocumentInterface", Version = "1.0")]

namespace CleanArchitecture.SingleFiles.Domain.Repositories.Documents
{
    public interface IMongoLineDocument
    {
        string Id { get; }
        string Name { get; }
    }
}