using System;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace EntityFrameworkCore.Postgres.Domain.Entities.ExplicitKeys
{
    public class PK_B_CompositeKey
    {
        public Guid CompositeKeyA { get; set; }

        public Guid CompositeKeyB { get; set; }
    }
}