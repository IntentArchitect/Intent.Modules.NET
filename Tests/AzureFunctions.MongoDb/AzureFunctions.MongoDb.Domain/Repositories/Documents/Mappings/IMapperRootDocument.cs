using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.MongoDb.MongoDbDocumentInterface", Version = "1.0")]

namespace AzureFunctions.MongoDb.Domain.Repositories.Documents.Mappings
{
    public interface IMapperRootDocument
    {
        string Id { get; }
        string No { get; }
        IEnumerable<string> MapAggChildrenIds { get; }
        string MapAggPeerId { get; }
        IEnumerable<string> MapperM2MSIds { get; }
        IMapCompChildDocument MapCompChild { get; }
        IMapCompOptionalDocument? MapCompOptional { get; }
    }
}