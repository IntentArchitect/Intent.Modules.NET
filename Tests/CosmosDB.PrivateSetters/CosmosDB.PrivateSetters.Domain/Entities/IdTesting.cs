using System;
using System.Collections.Generic;
using CosmosDB.PrivateSetters.Domain.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace CosmosDB.PrivateSetters.Domain.Entities
{
    public class IdTesting : IHasDomainEvent
    {
        private string? _identifier;

        public string Identifier
        {
            get => _identifier ??= Guid.NewGuid().ToString();
            private set => _identifier = value;
        }

        public string Id { get; private set; }

        public List<DomainEvent> DomainEvents { get; set; } = new List<DomainEvent>();
    }
}