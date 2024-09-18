using System;
using System.Collections.Generic;
using CosmosDB.Domain.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace CosmosDB.Domain.Entities
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
            set => _identifier = value;
        }

        public ClientType Type { get; set; }

        public string Name { get; set; }

        public List<DomainEvent> DomainEvents { get; set; } = new List<DomainEvent>();

        public void Update(ClientType type, string name)
        {
            Type = type;
            Name = name;
        }
    }
}