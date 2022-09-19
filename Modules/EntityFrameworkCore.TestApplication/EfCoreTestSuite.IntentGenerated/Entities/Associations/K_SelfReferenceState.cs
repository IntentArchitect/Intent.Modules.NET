using System;
using System.Collections.Generic;
using EfCoreTestSuite.IntentGenerated.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntityState", Version = "1.0")]

namespace EfCoreTestSuite.IntentGenerated.Entities.Associations
{

    public partial class K_SelfReference : IK_SelfReference
    {

        public Guid Id
        { get; set; }


        public Guid? K_SelfReferenceAssociationId { get; set; }

        public virtual K_SelfReference K_SelfReferenceAssociation
        { get; set; }

        IK_SelfReference IK_SelfReference.K_SelfReferenceAssociation
        {
            get => K_SelfReferenceAssociation;
            set => K_SelfReferenceAssociation = (K_SelfReference)value;
        }


    }
}
