using System;
using System.Collections.Generic;
using CosmosDB.PrivateSetters.Domain.Common;
using CosmosDB.PrivateSetters.Domain.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace CosmosDB.PrivateSetters.Domain.Entities
{
    public class Client : IHasDomainEvent, ISoftDelete
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

        void ISoftDelete.SetDeleted(bool isDeleted)
        {
            IsDeleted = isDeleted;
        }
    }
}