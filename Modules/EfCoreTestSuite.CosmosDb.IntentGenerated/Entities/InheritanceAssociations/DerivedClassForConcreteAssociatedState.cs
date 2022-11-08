using System;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntityState", Version = "1.0")]

namespace EfCoreTestSuite.CosmosDb.IntentGenerated.Entities.InheritanceAssociations
{
    public partial class DerivedClassForConcreteAssociated : IDerivedClassForConcreteAssociated
    {
        public Guid Id { get; set; }

        public Guid DerivedClassForConcreteId { get; set; }

        public string PartitionKey { get; set; }

        public string AssociatedField { get; set; }

        public virtual DerivedClassForConcrete DerivedClassForConcrete { get; set; }

        IDerivedClassForConcrete IDerivedClassForConcreteAssociated.DerivedClassForConcrete
        {
            get => DerivedClassForConcrete;
            set => DerivedClassForConcrete = (DerivedClassForConcrete)value;
        }
    }
}