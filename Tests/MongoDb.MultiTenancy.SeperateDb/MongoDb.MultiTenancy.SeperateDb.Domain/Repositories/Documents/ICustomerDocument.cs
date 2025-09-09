using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.MongoDb.MongoDbDocumentInterface", Version = "1.0")]

namespace MongoDb.MultiTenancy.SeperateDb.Domain.Repositories.Documents
{
    public interface ICustomerDocument
    {
        string Id { get; }
        string Name { get; }
    }
}