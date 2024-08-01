using System;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace EntityFrameworkCore.Postgres.Domain.Entities.Associations
{
    public class P_SourceNameDiffDependent
    {
        public Guid Id { get; set; }

        public Guid SourceNameDiffId { get; set; }
    }
}