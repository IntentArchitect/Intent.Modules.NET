using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.MongoDb.MongoDbDocumentInterface", Version = "1.0")]

namespace Entities.PrivateSetters.MongoDb.Domain.Repositories.Documents
{
    public interface IInvoiceDocument
    {
        string Id { get; }
        DateTime Date { get; }
        IEnumerable<string> TagsIds { get; }
        IEnumerable<ILineDocument> Lines { get; }
    }
}