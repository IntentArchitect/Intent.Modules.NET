using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.MongoDb.MongoDbDocumentInterface", Version = "1.0")]

namespace GraphQL.MongoDb.TestApplication.Domain.Repositories.Documents
{
    public interface IUserDocument
    {
        string Id { get; }
        string Name { get; }
        string Surname { get; }
        string Email { get; }
        IEnumerable<IAssignedPrivilegeDocument> AssignedPrivileges { get; }
    }
}