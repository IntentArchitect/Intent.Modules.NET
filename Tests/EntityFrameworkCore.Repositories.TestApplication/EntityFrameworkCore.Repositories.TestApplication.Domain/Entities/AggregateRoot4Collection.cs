using System;
using System.Collections.Generic;
using EntityFrameworkCore.Repositories.TestApplication.Domain.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace EntityFrameworkCore.Repositories.TestApplication.Domain.Entities
{
    public class AggregateRoot4Collection : IHasDomainEvent
    {
        public Guid Id { get; set; }

        public List<DomainEvent> DomainEvents { get; set; } = [];

        public Guid? AggregateRoot4AggNullableId { get; set; }
    }
}