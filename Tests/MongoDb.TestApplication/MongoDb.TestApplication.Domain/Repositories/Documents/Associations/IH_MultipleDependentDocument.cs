using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.MongoDb.MongoDbDocumentInterface", Version = "1.0")]

namespace MongoDb.TestApplication.Domain.Repositories.Documents.Associations
{
    public interface IH_MultipleDependentDocument
    {
        string Id { get; }
        string Attribute { get; }
        string? HOptionalaggregatenavId { get; }
    }
}