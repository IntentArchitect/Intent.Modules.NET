using System;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntityState", Version = "1.0")]

namespace EfCoreTestSuite.CosmosDb.IntentGenerated.Entities.Associations
{
    public partial class J_MultipleAggregate : IJ_MultipleAggregate
    {
        public Guid Id { get; set; }

        public Guid J_RequiredDependentId { get; set; }

        public string PartitionKey { get; set; }

        public string MultipleAggrAttr { get; set; }

        public virtual J_RequiredDependent J_RequiredDependent { get; set; }

        IJ_RequiredDependent IJ_MultipleAggregate.J_RequiredDependent
        {
            get => J_RequiredDependent;
            set => J_RequiredDependent = (J_RequiredDependent)value;
        }
    }
}