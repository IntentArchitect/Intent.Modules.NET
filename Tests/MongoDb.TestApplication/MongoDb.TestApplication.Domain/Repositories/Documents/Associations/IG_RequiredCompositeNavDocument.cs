using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.MongoDb.MongoDbDocumentInterface", Version = "1.0")]

namespace MongoDb.TestApplication.Domain.Repositories.Documents.Associations
{
    public interface IG_RequiredCompositeNavDocument
    {
        string Id { get; }
        string Attribute { get; }
        IEnumerable<IG_MultipleDependentDocument> G_MultipleDependents { get; }
    }
}