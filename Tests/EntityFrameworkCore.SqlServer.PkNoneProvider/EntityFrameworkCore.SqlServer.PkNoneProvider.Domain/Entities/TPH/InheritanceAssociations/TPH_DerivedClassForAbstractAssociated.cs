using System;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace EntityFrameworkCore.SqlServer.PkNoneProvider.Domain.Entities.TPH.InheritanceAssociations
{
    public class TPH_DerivedClassForAbstractAssociated
    {
        public TPH_DerivedClassForAbstractAssociated()
        {
            AssociatedField = null!;
            DerivedClassForAbstract = null!;
        }

        public Guid Id { get; set; }

        public string AssociatedField { get; set; }

        public Guid DerivedClassForAbstractId { get; set; }

        public virtual TPH_DerivedClassForAbstract DerivedClassForAbstract { get; set; }
    }
}