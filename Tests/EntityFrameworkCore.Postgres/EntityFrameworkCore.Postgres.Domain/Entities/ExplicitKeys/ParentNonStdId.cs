using System;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace EntityFrameworkCore.Postgres.Domain.Entities.ExplicitKeys
{
    public class ParentNonStdId
    {
        public ParentNonStdId()
        {
            Desc = null!;
            ChildNonStdId = null!;
        }
        public Guid MyId { get; set; }

        public string Desc { get; set; }

        public virtual ChildNonStdId ChildNonStdId { get; set; }
    }
}