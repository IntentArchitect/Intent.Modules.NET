using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocumentInterface", Version = "1.0")]

namespace CosmosDB.PrivateSetters.Domain.Repositories.Documents
{
    public interface IEntityOfTDocument<T>
    {
        string Id { get; }
        T GenericAttribute { get; }
    }
}