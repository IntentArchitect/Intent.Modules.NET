using System;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntityState", Version = "1.0")]

namespace EfCoreTestSuite.CosmosDb.IntentGenerated.Entities.InheritanceAssociations
{
    public partial class ConcreteBaseClassAssociated : IConcreteBaseClassAssociated
    {
        public Guid Id { get; set; }

        public Guid ConcreteBaseClassId { get; set; }

        public string PartitionKey { get; set; }

        public string AssociatedField { get; set; }

        public virtual ConcreteBaseClass ConcreteBaseClass { get; set; }

        IConcreteBaseClass IConcreteBaseClassAssociated.ConcreteBaseClass
        {
            get => ConcreteBaseClass;
            set => ConcreteBaseClass = (ConcreteBaseClass)value;
        }
    }
}