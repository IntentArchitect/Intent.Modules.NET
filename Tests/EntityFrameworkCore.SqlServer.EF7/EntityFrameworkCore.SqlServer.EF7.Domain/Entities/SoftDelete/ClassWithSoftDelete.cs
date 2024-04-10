using System;
using EntityFrameworkCore.SqlServer.EF7.Domain.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace EntityFrameworkCore.SqlServer.EF7.Domain.Entities.SoftDelete
{
    public class ClassWithSoftDelete : ISoftDelete
    {
        public Guid Id { get; set; }

        public string Attribute1 { get; set; }

        public bool IsDeleted { get; set; }
    }
}