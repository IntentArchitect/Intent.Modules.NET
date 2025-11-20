using System;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace EntityFrameworkCore.SqlServer.EF10.Domain.Entities.TPC.InheritanceAssociations
{
    public class TPC_DerivedClassForConcreteAssociated
    {
        public TPC_DerivedClassForConcreteAssociated()
        {
            AssociatedField = null!;
            DerivedClassForConcrete = null!;
        }
        public Guid Id { get; set; }

        public string AssociatedField { get; set; }

        public Guid DerivedClassForConcreteId { get; set; }

        public virtual TPC_DerivedClassForConcrete DerivedClassForConcrete { get; set; }
    }
}