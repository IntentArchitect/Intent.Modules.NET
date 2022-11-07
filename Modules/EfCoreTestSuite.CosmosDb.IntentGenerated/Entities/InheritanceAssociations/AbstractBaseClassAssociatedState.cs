using System;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntityState", Version = "1.0")]

namespace EfCoreTestSuite.CosmosDb.IntentGenerated.Entities.InheritanceAssociations
{
    public partial class AbstractBaseClassAssociated : IAbstractBaseClassAssociated
    {
        public Guid Id { get; set; }

        public Guid AbstractBaseClassId { get; set; }

        public string PartitionKey { get; set; }

        public string AssociatedField { get; set; }

        public virtual AbstractBaseClass AbstractBaseClass { get; set; }

        IAbstractBaseClass IAbstractBaseClassAssociated.AbstractBaseClass
        {
            get => AbstractBaseClass;
            set => AbstractBaseClass = (AbstractBaseClass)value;
        }
    }
}