using CosmosDB.PrivateSetters.Domain.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocumentInterface", Version = "1.0")]

namespace CosmosDB.PrivateSetters.Domain.Repositories.Documents
{
    public interface IClientDocument : ISoftDeleteReadOnly
    {
        string Identifier { get; }
        ClientType Type { get; }
        string Name { get; }
    }
}