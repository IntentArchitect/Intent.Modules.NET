using System;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntityState", Version = "1.0")]

namespace EfCoreTestSuite.CosmosDb.IntentGenerated.Entities.Polymorphic
{
    public partial class Poly_BaseClassNonAbstract : IPoly_BaseClassNonAbstract
    {
        public Guid Id { get; set; }

        public string BaseField { get; set; }
    }
}