using System;
using EntityFrameworkCore.MySql.Domain.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace EntityFrameworkCore.MySql.Domain.Entities.SoftDelete
{
    public class ClassWithSoftDelete : ISoftDelete
    {
        public Guid Id { get; set; }

        public string Attribute1 { get; set; }

        public bool IsDeleted { get; set; }
    }
}