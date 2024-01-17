using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocumentInterface", Version = "1.0")]

namespace AdvancedMappingCrud.Cosmos.Tests.Domain.Repositories.Documents
{
    public interface IOrderItemDocument
    {
        string Id { get; }
        int Quantity { get; }
        decimal Amount { get; }
        string ProductId { get; }
    }
}