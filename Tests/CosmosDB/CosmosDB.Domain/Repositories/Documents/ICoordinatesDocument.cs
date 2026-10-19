using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBValueObjectDocumentInterface", Version = "1.0")]

namespace CosmosDB.Domain.Repositories.Documents
{
    public interface ICoordinatesDocument
    {
        decimal Latitude { get; }
        decimal Longitude { get; }
    }
}