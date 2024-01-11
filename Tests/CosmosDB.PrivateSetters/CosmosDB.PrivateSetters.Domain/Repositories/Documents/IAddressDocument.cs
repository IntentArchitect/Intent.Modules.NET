using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBValueObjectDocumentInterface", Version = "1.0")]

namespace CosmosDB.PrivateSetters.Domain.Repositories.Documents
{
    public interface IAddressDocument
    {
        string Line1 { get; }
        string Line2 { get; }
        string City { get; }
        string PostalAddress { get; }
    }
}