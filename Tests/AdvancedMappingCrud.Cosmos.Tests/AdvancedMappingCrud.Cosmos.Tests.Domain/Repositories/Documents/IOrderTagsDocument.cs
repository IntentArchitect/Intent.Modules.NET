using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBValueObjectDocumentInterface", Version = "1.0")]

namespace AdvancedMappingCrud.Cosmos.Tests.Domain.Repositories.Documents
{
    public interface IOrderTagsDocument
    {
        string Name { get; }
        string Value { get; }
    }
}