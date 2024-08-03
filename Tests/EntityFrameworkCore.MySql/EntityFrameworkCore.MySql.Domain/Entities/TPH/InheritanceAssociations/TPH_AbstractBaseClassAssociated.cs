using System;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace EntityFrameworkCore.MySql.Domain.Entities.TPH.InheritanceAssociations
{
    public class TPH_AbstractBaseClassAssociated
    {
        public Guid Id { get; set; }

        public string AssociatedField { get; set; }

        public Guid AbstractBaseClassId { get; set; }

        public virtual TPH_AbstractBaseClass AbstractBaseClass { get; set; }
    }
}