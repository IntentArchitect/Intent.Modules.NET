using System;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace EntityFrameworkCore.SqlServer.EF8.Domain.Entities.TPC.InheritanceAssociations
{
    public class TPC_DerivedClassForAbstractAssociated
    {
        public TPC_DerivedClassForAbstractAssociated()
        {
            AssociatedField = null!;
            DerivedClassForAbstract = null!;
        }
        public Guid Id { get; set; }

        public string AssociatedField { get; set; }

        public Guid DerivedClassForAbstractId { get; set; }

        public virtual TPC_DerivedClassForAbstract DerivedClassForAbstract { get; set; }
    }
}