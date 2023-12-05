using System;
using System.Collections.Generic;
using CosmosDB.EntityInterfaces.Domain.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace CosmosDB.EntityInterfaces.Domain.Entities
{
    public class IdTesting : IIdTesting, IHasDomainEvent
    {
        private string? _identifier;

        public string Identifier
        {
            get => _identifier ??= Guid.NewGuid().ToString();
            set => _identifier = value;
        }

        public string Id { get; set; }

        public List<DomainEvent> DomainEvents { get; set; } = new List<DomainEvent>();
    }
}