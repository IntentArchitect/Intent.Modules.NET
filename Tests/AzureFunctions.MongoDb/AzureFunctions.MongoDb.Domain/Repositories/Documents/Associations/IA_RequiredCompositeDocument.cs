using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.MongoDb.MongoDbDocumentInterface", Version = "1.0")]

namespace AzureFunctions.MongoDb.Domain.Repositories.Documents.Associations
{
    public interface IA_RequiredCompositeDocument
    {
        string Id { get; }
        string ReqCompAttribute { get; }
        IA_OptionalDependentDocument? A_OptionalDependent { get; }
    }
}