using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.MongoDb.MongoDbDocumentInterface", Version = "1.0")]

namespace AzureFunctions.MongoDb.Domain.Repositories.Documents.Mappings
{
    public interface IMapAggPeerDocument
    {
        string Id { get; }
        string PeerAtt { get; }
        string MapAggPeerAggId { get; }
        string MapMapMeId { get; }
        IMapPeerCompChildDocument MapPeerCompChild { get; }
    }
}