using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntityState", Version = "1.0")]

namespace EfCoreTestSuite.CosmosDb.IntentGenerated.Entities.Polymorphic
{

    public partial class Poly_BaseClassNonAbstract : Poly_RootAbstract, IPoly_BaseClassNonAbstract
    {

        public string BaseField { get; set; }


        public Guid? Poly_SecondLevelId { get; set; }
    }
}
