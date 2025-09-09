using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.MongoDb.MongoDbDocumentInterface", Version = "1.0")]

namespace AzureFunctions.MongoDb.Domain.Repositories.Documents.Associations
{
    public interface ID_MultipleDependentDocument
    {
        string Id { get; }
        string Attribute { get; }
        IEnumerable<string> DOptionalAggregatesIds { get; }
    }
}