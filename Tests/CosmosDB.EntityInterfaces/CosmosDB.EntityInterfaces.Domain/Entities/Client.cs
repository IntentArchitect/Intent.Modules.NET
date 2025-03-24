using System;
using System.Collections.Generic;
using CosmosDB.EntityInterfaces.Domain.Common;
using CosmosDB.EntityInterfaces.Domain.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace CosmosDB.EntityInterfaces.Domain.Entities
{
    public class Client : IClient, IHasDomainEvent, ISoftDelete
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
            set => _identifier = value;
        }

        public ClientType ClientType { get; set; }

        public string Name { get; set; }

        public bool IsDeleted { get; set; }

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