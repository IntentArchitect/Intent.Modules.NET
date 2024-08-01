using System;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace EntityFrameworkCore.Postgres.Domain.Entities.TPT.InheritanceAssociations
{
    public class TPT_FkBaseClassAssociated
    {
        public Guid Id { get; set; }

        public string AssociatedField { get; set; }

        public Guid FkBaseClassCompositeKeyA { get; set; }

        public Guid FkBaseClassCompositeKeyB { get; set; }

        public virtual TPT_FkBaseClass FkBaseClass { get; set; }
    }
}