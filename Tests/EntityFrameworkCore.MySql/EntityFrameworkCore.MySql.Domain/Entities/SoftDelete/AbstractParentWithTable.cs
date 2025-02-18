using System;
using EntityFrameworkCore.MySql.Domain.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace EntityFrameworkCore.MySql.Domain.Entities.SoftDelete
{
    public abstract class AbstractParentWithTable : ISoftDelete
    {
        public AbstractParentWithTable()
        {
            Name = null!;
        }

        public Guid Id { get; set; }

        public bool IsDeleted { get; set; }

        public string Name { get; set; }

        void ISoftDelete.SetDeleted(bool isDeleted)
        {
            IsDeleted = isDeleted;
        }
    }
}