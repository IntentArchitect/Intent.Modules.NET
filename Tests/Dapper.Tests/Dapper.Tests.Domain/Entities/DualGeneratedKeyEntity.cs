using System;
using System.Collections.Generic;
using Dapper.Tests.Domain.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace Dapper.Tests.Domain.Entities
{
    public class DualGeneratedKeyEntity : IHasDomainEvent
    {
        public DualGeneratedKeyEntity()
        {
            Description = null!;
        }

        public Guid KeyPartA { get; set; }

        public Guid KeyPartB { get; set; }

        public string Description { get; set; }

        public List<DomainEvent> DomainEvents { get; set; } = [];
    }
}