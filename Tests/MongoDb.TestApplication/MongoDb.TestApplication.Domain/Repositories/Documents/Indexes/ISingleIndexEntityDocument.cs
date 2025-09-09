using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.MongoDb.MongoDbDocumentInterface", Version = "1.0")]

namespace MongoDb.TestApplication.Domain.Repositories.Documents.Indexes
{
    public interface ISingleIndexEntityDocument
    {
        string Id { get; }
        string SomeField { get; }
        string SingleIndex { get; }
    }
}