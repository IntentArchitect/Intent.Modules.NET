using System;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace EntityFrameworkCore.MySql.Domain.Entities.TPT.InheritanceAssociations
{
    public class TPT_DerivedClassForConcreteAssociated
    {
        public TPT_DerivedClassForConcreteAssociated()
        {
            AssociatedField = null!;
            DerivedClassForConcrete = null!;
        }
        public Guid Id { get; set; }

        public string AssociatedField { get; set; }

        public Guid DerivedClassForConcreteId { get; set; }

        public virtual TPT_DerivedClassForConcrete DerivedClassForConcrete { get; set; }
    }
}