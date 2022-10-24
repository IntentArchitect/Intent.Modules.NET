using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntityState", Version = "1.0")]

namespace EfCoreTestSuite.CosmosDb.IntentGenerated.Entities.InheritanceAssociations
{

    public partial class DerivedClassForAbstractAssociated : IDerivedClassForAbstractAssociated
    {

        public Guid Id { get; set; }

        public string AssociatedField { get; set; }

        public string PartitionKey { get; set; }


        public Guid DerivedClassForAbstractId { get; set; }

        public virtual DerivedClassForAbstract DerivedClassForAbstract { get; set; }

        IDerivedClassForAbstract IDerivedClassForAbstractAssociated.DerivedClassForAbstract
        {
            get => DerivedClassForAbstract;
            set => DerivedClassForAbstract = (DerivedClassForAbstract)value;
        }


    }
}
