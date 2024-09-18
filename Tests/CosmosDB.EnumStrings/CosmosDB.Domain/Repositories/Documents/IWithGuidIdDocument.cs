using System;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocumentInterface", Version = "1.0")]

namespace CosmosDB.Domain.Repositories.Documents
{
    public interface IWithGuidIdDocument
    {
        string Id { get; }
        string Field { get; }
    }
}