using System;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace EntityFrameworkCore.SqlServer.EF8.Domain.Entities.TPT.InheritanceAssociations
{
    public class TPT_FkAssociatedClass
    {
        public Guid Id { get; set; }

        public string AssociatedField { get; set; }

        public Guid FkDerivedClassCompositeKeyA { get; set; }

        public Guid FkDerivedClassCompositeKeyB { get; set; }

        public virtual TPT_FkDerivedClass FkDerivedClass { get; set; }
    }
}