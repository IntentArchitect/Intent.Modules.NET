using System;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocumentInterface", Version = "1.0")]

namespace BasicAuditing.CustomUserId.Tests.Domain.Repositories.Documents
{
    public interface IAccountDocument
    {
        string Id { get; }
        string Name { get; }
        Guid CreatedBy { get; }
        DateTimeOffset CreatedDate { get; }
        Guid? UpdatedBy { get; }
        DateTimeOffset? UpdatedDate { get; }
    }
}