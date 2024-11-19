using System;
using System.Collections.Generic;
using EntityFrameworkCore.MultiDbContext.WithDefaultDbContext.Domain.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace EntityFrameworkCore.MultiDbContext.WithDefaultDbContext.Domain.Entities
{
    public class EntityAlternate : IHasDomainEvent
    {
        public EntityAlternate()
        {
            Message = null!;
        }
        public Guid Id { get; set; }

        public string Message { get; set; }

        public List<DomainEvent> DomainEvents { get; set; } = [];
    }
}