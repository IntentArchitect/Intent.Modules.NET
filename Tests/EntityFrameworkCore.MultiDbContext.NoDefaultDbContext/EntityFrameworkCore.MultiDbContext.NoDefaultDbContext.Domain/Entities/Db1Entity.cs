using System;
using System.Collections.Generic;
using EntityFrameworkCore.MultiDbContext.NoDefaultDbContext.Domain.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace EntityFrameworkCore.MultiDbContext.NoDefaultDbContext.Domain.Entities
{
    public class Db1Entity : IHasDomainEvent
    {
        public Db1Entity()
        {
            Message = null!;
        }
        public Guid Id { get; set; }

        public string Message { get; set; }

        public List<DomainEvent> DomainEvents { get; set; } = [];
    }
}