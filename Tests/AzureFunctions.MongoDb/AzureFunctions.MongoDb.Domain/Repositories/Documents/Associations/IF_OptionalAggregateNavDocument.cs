using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.MongoDb.MongoDbDocumentInterface", Version = "1.0")]

namespace AzureFunctions.MongoDb.Domain.Repositories.Documents.Associations
{
    public interface IF_OptionalAggregateNavDocument
    {
        string Id { get; }
        string Attribute { get; }
        string? FOptionalDependentId { get; }
    }
}