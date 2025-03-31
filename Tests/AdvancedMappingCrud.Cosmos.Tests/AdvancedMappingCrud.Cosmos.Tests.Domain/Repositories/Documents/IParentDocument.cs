using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocumentInterface", Version = "1.0")]

namespace AdvancedMappingCrud.Cosmos.Tests.Domain.Repositories.Documents
{
    public interface IParentDocument
    {
        string Id { get; }
        string Name { get; }
        IReadOnlyList<IChildDocument>? Children { get; }
        IParentDetailsDocument? ParentDetails { get; }
    }
}