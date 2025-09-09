using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.MongoDb.MongoDbDocumentInterface", Version = "1.0")]

namespace Entities.PrivateSetters.MongoDb.Domain.Repositories.Documents
{
    public interface ITagDocument
    {
        string Id { get; }
        string Name { get; }
    }
}