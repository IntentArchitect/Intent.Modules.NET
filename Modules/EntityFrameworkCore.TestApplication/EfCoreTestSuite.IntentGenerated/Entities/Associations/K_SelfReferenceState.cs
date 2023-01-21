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

        public Guid Id { get; set; }

        public string SelfRefAttr { get; set; }

        public Guid? KSelfReferenceAssociationId { get; set; }

        public virtual K_SelfReference KSelfReferenceAssociation { get; set; }

        IK_SelfReference IK_SelfReference.KSelfReferenceAssociation
        {
            get => KSelfReferenceAssociation;
            set => KSelfReferenceAssociation = (K_SelfReference)value;
        }


    }
}
