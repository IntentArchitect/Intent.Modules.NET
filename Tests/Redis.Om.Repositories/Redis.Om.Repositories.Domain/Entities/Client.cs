using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;
using Redis.Om.Repositories.Domain.Common;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace Redis.Om.Repositories.Domain.Entities
{
    public class Client : IHasDomainEvent
    {
        private string? _id;
        public Client()
        {
        }

        public Client(ClientType type, string name)
        {
            Type = type;
            Name = name;
        }

        public string Id
        {
            get => _id;
            set => _id = value;
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