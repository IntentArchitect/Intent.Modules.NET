using System;
using EntityFrameworkCore.SqlServer.EF8.Domain.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace EntityFrameworkCore.SqlServer.EF8.Domain.Entities.SoftDelete
{
    public class ClassWithSoftDelete : ISoftDelete
    {
        public Guid Id { get; set; }

        public string Attribute1 { get; set; }

        public bool IsDeleted { get; set; }
    }
}