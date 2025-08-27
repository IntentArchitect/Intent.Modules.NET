using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.MongoDb.MongoDbDocumentInterface", Version = "1.0")]

namespace Entities.PrivateSetters.MongoDb.Domain.Repositories.Documents
{
    public interface ILineDocument
    {
        string Id { get; }
        string Description { get; }
        int Quantity { get; }
    }
}