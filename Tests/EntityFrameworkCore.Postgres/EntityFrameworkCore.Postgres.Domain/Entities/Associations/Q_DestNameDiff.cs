using System;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace EntityFrameworkCore.Postgres.Domain.Entities.Associations
{
    public class Q_DestNameDiff
    {
        public Guid Id { get; set; }

        public virtual Q_DestNameDiffDependent DestNameDiff { get; set; }
    }
}