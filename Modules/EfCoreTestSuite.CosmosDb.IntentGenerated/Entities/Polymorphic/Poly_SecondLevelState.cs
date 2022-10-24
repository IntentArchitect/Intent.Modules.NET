using System;
using System.Collections.Generic;
using System.Linq;
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

        public virtual ICollection<Poly_BaseClassNonAbstract> Poly_BaseClassNonAbstracts { get; set; } = new List<Poly_BaseClassNonAbstract>();

        ICollection<IPoly_BaseClassNonAbstract> IPoly_SecondLevel.Poly_BaseClassNonAbstracts
        {
            get => Poly_BaseClassNonAbstracts.CreateWrapper<IPoly_BaseClassNonAbstract, Poly_BaseClassNonAbstract>();
            set => Poly_BaseClassNonAbstracts = value.Cast<Poly_BaseClassNonAbstract>().ToList();
        }


    }
}
