using System;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace EntityFrameworkCore.SqlServer.EF7.Domain.Entities.ExplicitKeys
{
    public class FK_B_CompositeForeignKey
    {
        public FK_B_CompositeForeignKey()
        {
            PK_CompositeKey = null!;
        }
        public Guid Id { get; set; }

        public Guid PK_CompositeKeyCompositeKeyA { get; set; }

        public Guid PK_CompositeKeyCompositeKeyB { get; set; }

        public virtual PK_B_CompositeKey PK_CompositeKey { get; set; }
    }
}