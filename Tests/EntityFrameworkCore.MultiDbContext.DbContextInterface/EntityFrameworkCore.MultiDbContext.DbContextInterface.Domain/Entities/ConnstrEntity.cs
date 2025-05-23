using System;
using System.Collections.Generic;
using EntityFrameworkCore.MultiDbContext.DbContextInterface.Domain.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace EntityFrameworkCore.MultiDbContext.DbContextInterface.Domain.Entities
{
    public class ConnstrEntity : IHasDomainEvent
    {
        public ConnstrEntity()
        {
            Message = null!;
        }
        public Guid Id { get; set; }

        public string Message { get; set; }

        public List<DomainEvent> DomainEvents { get; set; } = [];
    }
}