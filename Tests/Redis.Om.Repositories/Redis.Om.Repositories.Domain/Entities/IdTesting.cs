using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;
using Redis.Om.Repositories.Domain.Common;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace Redis.Om.Repositories.Domain.Entities
{
    public class IdTesting : IHasDomainEvent
    {
        private string? _identifier;
        public IdTesting()
        {
            Identifier = null!;
            Id = null!;
        }
        public string Identifier
        {
            get => _identifier;
            set => _identifier = value;
        }

        public string Id { get; set; }

        public List<DomainEvent> DomainEvents { get; set; } = [];
    }
}