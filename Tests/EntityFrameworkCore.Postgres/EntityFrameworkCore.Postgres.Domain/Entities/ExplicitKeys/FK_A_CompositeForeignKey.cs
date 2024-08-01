using System;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace EntityFrameworkCore.Postgres.Domain.Entities.ExplicitKeys
{
    public class FK_A_CompositeForeignKey
    {
        public Guid Id { get; set; }

        public Guid PK_A_CompositeKeyCompositeKeyA { get; set; }

        public Guid PK_A_CompositeKeyCompositeKeyB { get; set; }

        public virtual PK_A_CompositeKey PK_A_CompositeKey { get; set; }
    }
}