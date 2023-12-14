using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocumentInterface", Version = "1.0")]

namespace CosmosDB.PrivateSetters.Domain.Repositories.Documents
{
    public interface IOrderDocument
    {
        string Id { get; }
        string WarehouseId { get; }
        string RefNo { get; }
        DateTime OrderDate { get; }
        IReadOnlyList<IOrderItemDocument> OrderItems { get; }
    }
}