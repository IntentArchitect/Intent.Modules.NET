using System;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace EntityFrameworkCore.SqlServer.PkNoneProvider.Domain.Entities.TPT.InheritanceAssociations
{
    public class TPT_AbstractBaseClassAssociated
    {
        public TPT_AbstractBaseClassAssociated()
        {
            AssociatedField = null!;
            AbstractBaseClass = null!;
        }

        public Guid Id { get; set; }

        public string AssociatedField { get; set; }

        public Guid AbstractBaseClassId { get; set; }

        public virtual TPT_AbstractBaseClass AbstractBaseClass { get; set; }
    }
}