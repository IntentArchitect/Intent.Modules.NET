using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.MongoDb.MongoDbDocumentInterface", Version = "1.0")]

namespace GraphQL.MongoDb.TestApplication.Domain.Repositories.Documents
{
    public interface IAssignedPrivilegeDocument
    {
        string Id { get; }
        string PrivilegeId { get; }
    }
}