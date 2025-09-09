using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.MongoDb.MongoDbDocumentInterface", Version = "1.0")]

namespace MongoDb.TestApplication.Domain.Repositories.Documents.Associations
{
    public interface IC_RequireCompositeDocument
    {
        string Id { get; }
        string Attribute { get; }
        IEnumerable<IC_MultipleDependentDocument> C_MultipleDependents { get; }
    }
}