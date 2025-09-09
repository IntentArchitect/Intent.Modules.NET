using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.MongoDb.MongoDbDocumentInterface", Version = "1.0")]

namespace AzureFunctions.MongoDb.Domain.Repositories.Documents.NestedAssociations
{
    public interface IAggregateADocument
    {
        string Id { get; }
        string Attribute { get; }
        INestedCompositionADocument NestedCompositionA { get; }
    }
}