using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.MongoDb.MongoDbDocumentInterface", Version = "1.0")]

namespace MongoDb.TestApplication.Domain.Repositories.Documents.Indexes
{
    public interface IMultikeyIndexEntitySingleParentDocument
    {
        string Id { get; }
        string SomeField { get; }
        IMultikeyIndexEntitySingleChildDocument MultikeyIndexEntitySingleChild { get; }
    }
}