using System;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace EntityFrameworkCore.SqlServer.EF7.Domain.Entities.TPH.InheritanceAssociations
{
    public class TPH_FkBaseClassAssociated
    {
        public Guid Id { get; set; }

        public string AssociatedField { get; set; }

        public Guid FkBaseClassCompositeKeyA { get; set; }

        public Guid FkBaseClassCompositeKeyB { get; set; }

        public virtual TPH_FkBaseClass FkBaseClass { get; set; }
    }
}