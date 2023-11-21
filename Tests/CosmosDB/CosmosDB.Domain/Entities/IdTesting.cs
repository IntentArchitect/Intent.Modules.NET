using System;
using System.Collections.Generic;
using CosmosDB.Domain.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTagModeImplicit]

namespace CosmosDB.Domain.Entities
{
    public class IdTesting : IHasDomainEvent
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