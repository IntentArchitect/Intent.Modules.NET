using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocumentInterface", Version = "1.0")]

namespace AdvancedMappingCrud.Cosmos.Tests.Domain.Repositories.Documents
{
    public interface ICustomerDocument
    {
        string Id { get; }
        string Name { get; }
        string Surname { get; }
        bool IsActive { get; }
        IAddressDocument ShippingAddress { get; }
    }
}