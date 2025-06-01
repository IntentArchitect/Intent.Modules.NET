using System;
using System.Collections.Generic;
using EntityFrameworkCore.MultiDbContext.DbContextInterface.Domain.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace EntityFrameworkCore.MultiDbContext.DbContextInterface.Domain.Entities
{
    public class ConnstrDomainPackageAuditLog : IHasDomainEvent
    {
        public ConnstrDomainPackageAuditLog()
        {
            TableName = null!;
            Key = null!;
            ChangedBy = null!;
        }

        public int Id { get; set; }

        public string TableName { get; set; }

        public string Key { get; set; }

        public string? ColumnName { get; set; }

        public string? OldValue { get; set; }

        public string? NewValue { get; set; }

        public string ChangedBy { get; set; }

        public DateTimeOffset ChangedDate { get; set; }

        public List<DomainEvent> DomainEvents { get; set; } = [];
    }
}