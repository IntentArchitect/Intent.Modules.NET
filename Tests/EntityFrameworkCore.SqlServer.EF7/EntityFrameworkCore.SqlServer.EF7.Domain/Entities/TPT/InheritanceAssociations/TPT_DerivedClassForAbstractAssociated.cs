using System;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace EntityFrameworkCore.SqlServer.EF7.Domain.Entities.TPT.InheritanceAssociations
{
    public class TPT_DerivedClassForAbstractAssociated
    {
        public Guid Id { get; set; }

        public string AssociatedField { get; set; }

        public Guid DerivedClassForAbstractId { get; set; }

        public virtual TPT_DerivedClassForAbstract DerivedClassForAbstract { get; set; }
    }
}