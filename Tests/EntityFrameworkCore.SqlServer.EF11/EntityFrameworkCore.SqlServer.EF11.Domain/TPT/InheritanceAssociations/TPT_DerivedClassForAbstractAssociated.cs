using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace EntityFrameworkCore.SqlServer.EF11.Domain.TPT.InheritanceAssociations
{
    public class TPT_DerivedClassForAbstractAssociated
    {
        public TPT_DerivedClassForAbstractAssociated()
        {
            AssociatedField = null!;
            DerivedClassForAbstract = null!;
        }

        public Guid Id { get; set; }

        public string AssociatedField { get; set; }

        public Guid DerivedClassForAbstractId { get; set; }

        public virtual TPT_DerivedClassForAbstract DerivedClassForAbstract { get; set; }
    }
}