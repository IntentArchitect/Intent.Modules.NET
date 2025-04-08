using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocumentInterface", Version = "1.0")]

namespace AdvancedMappingCrud.Cosmos.Tests.Domain.Repositories.Documents
{
    public interface IParentDetailsDocument
    {
        string DetailsLine1 { get; }
        string DetailsLine2 { get; }
        IParentSubDetailsDocument? ParentSubDetails { get; }
        IReadOnlyList<IParentDetailsTagsDocument>? ParentDetailsTags { get; }
    }
}