using System;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntityState", Version = "1.0")]

namespace EfCoreTestSuite.CosmosDb.IntentGenerated.Entities.Polymorphic
{
    public partial class Poly_SecondLevel : IPoly_SecondLevel
    {
        public Guid Id { get; set; }

        public string SecondField { get; set; }

        public string PartitionKey { get; set; }

        public virtual Poly_BaseClassNonAbstract BaseClassNonAbstracts { get; set; }

        IPoly_BaseClassNonAbstract IPoly_SecondLevel.BaseClassNonAbstracts
        {
            get => BaseClassNonAbstracts;
            set => BaseClassNonAbstracts = (Poly_BaseClassNonAbstract)value;
        }
    }
}