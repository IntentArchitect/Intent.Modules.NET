using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocumentInterface", Version = "1.0")]

namespace CosmosDB.EnumStrings.Domain.Repositories.Documents
{
    public interface INestedEntityDocument
    {
        string Id { get; }
        string Name { get; }
        EnumExample EnumExample { get; }
        EnumExample? NullableEnumExample { get; }
        IEmbeddedObjectDocument EmbeddedObject2 { get; }
        IEmbeddedObjectDocument EmbeddedObject { get; }
    }
}