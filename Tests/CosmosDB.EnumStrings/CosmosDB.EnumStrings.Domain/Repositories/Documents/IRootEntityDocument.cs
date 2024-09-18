using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocumentInterface", Version = "1.0")]

namespace CosmosDB.EnumStrings.Domain.Repositories.Documents
{
    public interface IRootEntityDocument
    {
        string Id { get; }
        string Name { get; }
        EnumExample EnumExample { get; }
        EnumExample? NullableEnumExample { get; }
        IEmbeddedObjectDocument Embedded { get; }
        IReadOnlyList<INestedEntityDocument> NestedEntities { get; }
    }
}