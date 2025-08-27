using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.MongoDb.MongoDbDocumentInterface", Version = "1.0")]

namespace AzureFunctions.MongoDb.Domain.Repositories.Documents.Mappings
{
    public interface IMapAggPeerAggDocument
    {
        string Id { get; }
        string MapAggPeerAggAtt { get; }
        string MapAggPeerAggMoreId { get; }
    }
}