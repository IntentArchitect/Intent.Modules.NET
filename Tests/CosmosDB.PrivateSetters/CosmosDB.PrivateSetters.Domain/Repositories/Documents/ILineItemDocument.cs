using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocumentInterface", Version = "1.0")]

namespace CosmosDB.PrivateSetters.Domain.Repositories.Documents
{
    public interface ILineItemDocument
    {
        string Id { get; }
        string Description { get; }
        int Quantity { get; }
        string ProductId { get; }
        IReadOnlyList<string> Tags { get; }
    }
}