using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.MongoDb.MongoDbDocumentInterface", Version = "1.0")]

namespace AzureFunctions.MongoDb.Domain.Repositories.Documents.Indexes
{
    public interface ISingleIndexEntitySingleParentDocument
    {
        string Id { get; }
        string SomeField { get; }
        ISingleIndexEntitySingleChildDocument SingleIndexEntitySingleChild { get; }
    }
}