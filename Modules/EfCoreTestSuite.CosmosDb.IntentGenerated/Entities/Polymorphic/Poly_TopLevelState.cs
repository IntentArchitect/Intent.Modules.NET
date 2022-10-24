using System;
using System.Collections.Generic;
using System.Linq;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntityState", Version = "1.0")]

namespace EfCoreTestSuite.CosmosDb.IntentGenerated.Entities.Polymorphic
{

    public partial class Poly_TopLevel : IPoly_TopLevel
    {

        public Guid Id { get; set; }

        public string TopField { get; set; }

        public string PartitionKey { get; set; }

        public virtual ICollection<Poly_RootAbstract> Poly_RootAbstracts { get; set; } = new List<Poly_RootAbstract>();

        ICollection<IPoly_RootAbstract> IPoly_TopLevel.Poly_RootAbstracts
        {
            get => Poly_RootAbstracts.CreateWrapper<IPoly_RootAbstract, Poly_RootAbstract>();
            set => Poly_RootAbstracts = value.Cast<Poly_RootAbstract>().ToList();
        }


    }
}
