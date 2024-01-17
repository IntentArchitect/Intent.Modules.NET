using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocumentInterface", Version = "1.0")]

namespace AdvancedMappingCrud.Cosmos.Tests.Domain.Repositories.Documents
{
    public interface IOrderDocument
    {
        string Id { get; }
        string CustomerId { get; }
        string RefNo { get; }
        DateTime OrderDate { get; }
        OrderStatus OrderStatus { get; }
        IReadOnlyList<IOrderItemDocument> OrderItems { get; }
        IReadOnlyList<IOrderTagsDocument> OrderTags { get; }
    }
}