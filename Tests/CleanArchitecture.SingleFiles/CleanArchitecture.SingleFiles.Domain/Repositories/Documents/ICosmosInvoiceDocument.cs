using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocumentInterface", Version = "1.0")]

namespace CleanArchitecture.SingleFiles.Domain.Repositories.Documents
{
    public interface ICosmosInvoiceDocument
    {
        string Id { get; }
        string Description { get; }
        IReadOnlyList<ICosmosLineDocument> CosmosLines { get; }
    }
}