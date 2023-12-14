using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocumentInterface", Version = "1.0")]

namespace CosmosDB.EntityInterfaces.Domain.Repositories.Documents
{
    public interface IOrderItemDocument
    {
        string Id { get; }
        int Quantity { get; }
        string Description { get; }
        decimal Amount { get; }
    }
}