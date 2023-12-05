using System;
using System.Collections.Generic;
using CosmosDB.PrivateSetters.Domain.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace CosmosDB.PrivateSetters.Domain.Entities
{
    public class Client : IHasDomainEvent
    {
        private string? _identifier;

        public Client()
        {
        }

        public Client(ClientType type, string name)
        {
            Type = type;
            Name = name;
        }

        public string Identifier
        {
            get => _identifier ??= Guid.NewGuid().ToString();
            private set => _identifier = value;
        }

        public ClientType Type { get; private set; }

        public string Name { get; private set; }

        public List<DomainEvent> DomainEvents { get; set; } = new List<DomainEvent>();

        public void Update(ClientType type, string name)
        {
            Type = type;
            Name = name;
        }
    }
}