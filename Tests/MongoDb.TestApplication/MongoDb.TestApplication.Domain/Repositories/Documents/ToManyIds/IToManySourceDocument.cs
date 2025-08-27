using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.MongoDb.MongoDbDocumentInterface", Version = "1.0")]

namespace MongoDb.TestApplication.Domain.Repositories.Documents.ToManyIds
{
    public interface IToManySourceDocument
    {
        string Id { get; }
        IEnumerable<IToManyGuidDocument> ToManyGuids { get; }
        IEnumerable<IToManyIntDocument> ToManyInts { get; }
        IEnumerable<IToManyLongDocument> ToManyLongs { get; }
    }
}