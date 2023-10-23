using CosmosDB.EntityInterfaces.Domain.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntityInterface", Version = "1.0")]

namespace CosmosDB.EntityInterfaces.Domain.Entities
{
    public interface IClient : IHasDomainEvent
    {
        string Identifier { get; set; }

        ClientType Type { get; set; }

        string Name { get; set; }

        void Update(ClientType type, string name);
    }
}