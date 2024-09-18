using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBValueObjectDocumentInterface", Version = "1.0")]

namespace CosmosDB.EnumStrings.Domain.Repositories.Documents
{
    public interface IEmbeddedObjectDocument
    {
        string Name { get; }
        EnumExample EnumExample { get; }
        EnumExample? NullableEnumExample { get; }
    }
}