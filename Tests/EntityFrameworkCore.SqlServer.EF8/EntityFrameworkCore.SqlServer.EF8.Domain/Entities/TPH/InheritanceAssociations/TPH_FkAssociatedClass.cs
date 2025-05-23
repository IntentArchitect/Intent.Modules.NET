using System;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace EntityFrameworkCore.SqlServer.EF8.Domain.Entities.TPH.InheritanceAssociations
{
    public class TPH_FkAssociatedClass
    {
        public TPH_FkAssociatedClass()
        {
            AssociatedField = null!;
            FkDerivedClass = null!;
        }
        public Guid Id { get; set; }

        public string AssociatedField { get; set; }

        public Guid FkDerivedClassCompositeKeyA { get; set; }

        public Guid FkDerivedClassCompositeKeyB { get; set; }

        public virtual TPH_FkDerivedClass FkDerivedClass { get; set; }
    }
}