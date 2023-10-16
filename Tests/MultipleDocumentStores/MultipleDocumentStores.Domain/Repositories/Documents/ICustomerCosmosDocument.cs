using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocumentInterface", Version = "1.0")]

namespace MultipleDocumentStores.Domain.Repositories.Documents
{
    public interface ICustomerCosmosDocument
    {
        string Id { get; }
        string Name { get; }
    }
}