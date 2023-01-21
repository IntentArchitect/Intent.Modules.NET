using System;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntityState", Version = "1.0")]

namespace EfCoreTestSuite.CosmosDb.IntentGenerated.Entities.Associations
{
    public partial class J_MultipleAggregate : IJ_MultipleAggregate
    {
        public Guid Id { get; set; }

        public string PartitionKey { get; set; }

        public string MultipleAggrAttr { get; set; }

        public Guid JRequiredDependentId { get; set; }

        public virtual J_RequiredDependent JRequiredDependent { get; set; }

        IJ_RequiredDependent IJ_MultipleAggregate.JRequiredDependent
        {
            get => JRequiredDependent;
            set => JRequiredDependent = (J_RequiredDependent)value;
        }
    }
}