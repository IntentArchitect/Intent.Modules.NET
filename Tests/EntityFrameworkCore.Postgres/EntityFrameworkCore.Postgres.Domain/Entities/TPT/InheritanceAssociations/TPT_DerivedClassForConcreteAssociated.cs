using System;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace EntityFrameworkCore.Postgres.Domain.Entities.TPT.InheritanceAssociations
{
    public class TPT_DerivedClassForConcreteAssociated
    {
        public Guid Id { get; set; }

        public string AssociatedField { get; set; }

        public Guid DerivedClassForConcreteId { get; set; }

        public virtual TPT_DerivedClassForConcrete DerivedClassForConcrete { get; set; }
    }
}