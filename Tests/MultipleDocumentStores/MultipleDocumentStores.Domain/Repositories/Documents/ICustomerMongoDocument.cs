using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.MongoDb.MongoDbDocumentInterface", Version = "1.0")]

namespace MultipleDocumentStores.Domain.Repositories.Documents
{
    public interface ICustomerMongoDocument
    {
        string Id { get; }
        string Name { get; }
    }
}