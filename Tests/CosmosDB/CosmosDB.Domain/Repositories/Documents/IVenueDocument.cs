using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBValueObjectDocumentInterface", Version = "1.0")]

namespace CosmosDB.Domain.Repositories.Documents
{
    public interface IVenueDocument
    {
        string Name { get; }
        ICoordinatesDocument LocationAsAttribute { get; }
        ICoordinatesDocument Location { get; }
        ICoordinatesDocument? Backup { get; }
        IReadOnlyList<ICoordinatesDocument>? Entrances { get; }
    }
}