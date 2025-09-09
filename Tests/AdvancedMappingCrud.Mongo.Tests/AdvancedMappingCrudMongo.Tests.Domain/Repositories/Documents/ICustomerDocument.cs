using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.MongoDb.MongoDbDocumentInterface", Version = "1.0")]

namespace AdvancedMappingCrudMongo.Tests.Domain.Repositories.Documents
{
    public interface ICustomerDocument
    {
        string Id { get; }
        string Name { get; }
        string Surname { get; }
        string Email { get; }
    }
}