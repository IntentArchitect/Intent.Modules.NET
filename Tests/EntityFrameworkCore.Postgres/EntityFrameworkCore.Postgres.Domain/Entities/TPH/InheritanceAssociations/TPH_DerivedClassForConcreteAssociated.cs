using System;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace EntityFrameworkCore.Postgres.Domain.Entities.TPH.InheritanceAssociations
{
    public class TPH_DerivedClassForConcreteAssociated
    {
        public TPH_DerivedClassForConcreteAssociated()
        {
            AssociatedField = null!;
            DerivedClassForConcrete = null!;
        }
        public Guid Id { get; set; }

        public string AssociatedField { get; set; }

        public Guid DerivedClassForConcreteId { get; set; }

        public virtual TPH_DerivedClassForConcrete DerivedClassForConcrete { get; set; }
    }
}