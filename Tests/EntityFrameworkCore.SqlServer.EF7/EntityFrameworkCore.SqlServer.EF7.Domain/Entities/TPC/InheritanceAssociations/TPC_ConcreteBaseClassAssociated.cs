using System;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace EntityFrameworkCore.SqlServer.EF7.Domain.Entities.TPC.InheritanceAssociations
{
    public class TPC_ConcreteBaseClassAssociated
    {
        public TPC_ConcreteBaseClassAssociated()
        {
            AssociatedField = null!;
            ConcreteBaseClass = null!;
        }
        public Guid Id { get; set; }

        public string AssociatedField { get; set; }

        public Guid ConcreteBaseClassId { get; set; }

        public virtual TPC_ConcreteBaseClass ConcreteBaseClass { get; set; }
    }
}