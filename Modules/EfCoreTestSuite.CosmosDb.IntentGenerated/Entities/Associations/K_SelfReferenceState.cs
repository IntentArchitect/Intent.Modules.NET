using System;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntityState", Version = "1.0")]

namespace EfCoreTestSuite.CosmosDb.IntentGenerated.Entities.Associations
{
    public partial class K_SelfReference : IK_SelfReference
    {
        public Guid Id { get; set; }

        public string PartitionKey { get; set; }

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