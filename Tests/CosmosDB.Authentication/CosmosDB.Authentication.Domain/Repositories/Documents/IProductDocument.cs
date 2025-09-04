using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocumentInterface", Version = "1.0")]

namespace CosmosDB.Authentication.Domain.Repositories.Documents
{
    public interface IProductDocument
    {
        string Id { get; }
        string Name { get; }
        string Value { get; }
        int Qty { get; }
    }
}