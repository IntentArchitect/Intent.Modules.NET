using CosmosDB.EntityInterfaces.Domain.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocumentInterface", Version = "1.0")]

namespace CosmosDB.EntityInterfaces.Domain.Repositories.Documents
{
    public interface IClientDocument : ISoftDeleteReadOnly
    {
        string Identifier { get; }
        ClientType ClientType { get; }
        string Name { get; }
    }
}