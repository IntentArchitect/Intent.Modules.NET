using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;
using ValueObjects.Class.Domain.Common;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace ValueObjects.Class.Domain.Entities
{
    public class Journey : IHasDomainEvent
    {
        public Journey()
        {
            Name = null!;
            Route = null!;
        }

        public Guid Id { get; set; }

        public string Name { get; set; }

        public Route Route { get; set; }

        public List<DomainEvent> DomainEvents { get; set; } = [];
    }
}