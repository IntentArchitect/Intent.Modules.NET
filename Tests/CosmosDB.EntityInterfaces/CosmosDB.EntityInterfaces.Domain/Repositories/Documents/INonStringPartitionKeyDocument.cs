using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocumentInterface", Version = "1.0")]

namespace CosmosDB.EntityInterfaces.Domain.Repositories.Documents
{
    public interface INonStringPartitionKeyDocument
    {
        string Id { get; }
        string PartInt { get; }
        string Name { get; }
    }
}