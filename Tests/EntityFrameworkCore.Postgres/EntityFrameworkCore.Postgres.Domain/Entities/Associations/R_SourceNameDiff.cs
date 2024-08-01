using System;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace EntityFrameworkCore.Postgres.Domain.Entities.Associations
{
    public class R_SourceNameDiff
    {
        public Guid Id { get; set; }

        public virtual R_SourceNameDiffDependent R_SourceNameDiffDependent { get; set; }
    }
}