using System;
using System.Collections.Generic;
using CosmosDB.PrivateSetters.Domain.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace CosmosDB.PrivateSetters.Domain.Entities
{
    public class IdTesting : IHasDomainEvent
    {
        private List<Schedule> _schedules = [];
        private string? _identifier;
        public IdTesting()
        {
            Identifier = null!;
            Id = null!;
            Schedules = null!;
        }

        public string Identifier
        {
            get => _identifier ??= Guid.NewGuid().ToString();
            private set => _identifier = value;
        }

        public string Id { get; private set; }

        public IReadOnlyCollection<Schedule> Schedules
        {
            get => _schedules.AsReadOnly();
            private set => _schedules = new List<Schedule>(value);
        }

        public List<DomainEvent> DomainEvents { get; set; } = [];
    }
}