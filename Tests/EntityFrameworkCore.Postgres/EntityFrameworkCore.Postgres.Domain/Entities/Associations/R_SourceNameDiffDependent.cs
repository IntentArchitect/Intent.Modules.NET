using System;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace EntityFrameworkCore.Postgres.Domain.Entities.Associations
{
    public class R_SourceNameDiffDependent
    {
        public Guid Id { get; set; }
    }
}