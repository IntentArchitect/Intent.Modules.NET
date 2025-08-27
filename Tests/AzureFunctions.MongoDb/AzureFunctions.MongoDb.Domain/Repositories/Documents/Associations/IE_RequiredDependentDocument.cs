using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.MongoDb.MongoDbDocumentInterface", Version = "1.0")]

namespace AzureFunctions.MongoDb.Domain.Repositories.Documents.Associations
{
    public interface IE_RequiredDependentDocument
    {
        string Attribute { get; }
        IE_RequiredCompositeNavDocument E_RequiredCompositeNav { get; }
    }
}