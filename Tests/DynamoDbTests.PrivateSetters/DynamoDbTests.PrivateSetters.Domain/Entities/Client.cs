using DynamoDbTests.PrivateSetters.Domain.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace DynamoDbTests.PrivateSetters.Domain.Entities
{
    public class Client : IHasDomainEvent
    {
        private string? _identifier;

        public Client()
        {
        }

        public Client(ClientType type, string name)
        {
            ClientType = type;
            Name = name;
        }

        public string Identifier
        {
            get => _identifier ??= Guid.NewGuid().ToString();
            private set => _identifier = value;
        }

        public ClientType ClientType { get; private set; }

        public string Name { get; private set; }

        public bool IsDeleted { get; private set; }

        public List<DomainEvent> DomainEvents { get; set; } = [];

        public void Update(ClientType type, string name)
        {
            ClientType = type;
            Name = name;
        }
    }
}