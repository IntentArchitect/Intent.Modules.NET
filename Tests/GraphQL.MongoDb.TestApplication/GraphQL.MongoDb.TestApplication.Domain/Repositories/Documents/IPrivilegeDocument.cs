using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.MongoDb.MongoDbDocumentInterface", Version = "1.0")]

namespace GraphQL.MongoDb.TestApplication.Domain.Repositories.Documents
{
    public interface IPrivilegeDocument
    {
        string Id { get; }
        string Name { get; }
        string? Description { get; }
    }
}