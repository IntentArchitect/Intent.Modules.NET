using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.MongoDb.MongoDbDocumentInterface", Version = "1.0")]

namespace AzureFunctions.MongoDb.Domain.Repositories.Documents.Associations
{
    public interface IJ_MultipleAggregateDocument
    {
        string Id { get; }
        string Attribute { get; }
        IEnumerable<string> JMultipleDependentsIds { get; }
    }
}