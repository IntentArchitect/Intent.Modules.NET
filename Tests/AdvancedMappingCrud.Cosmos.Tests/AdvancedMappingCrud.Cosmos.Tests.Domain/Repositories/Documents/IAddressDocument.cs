using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBValueObjectDocumentInterface", Version = "1.0")]

namespace AdvancedMappingCrud.Cosmos.Tests.Domain.Repositories.Documents
{
    public interface IAddressDocument
    {
        string Line1 { get; }
        string Line2 { get; }
        string City { get; }
        string PostalCode { get; }
    }
}