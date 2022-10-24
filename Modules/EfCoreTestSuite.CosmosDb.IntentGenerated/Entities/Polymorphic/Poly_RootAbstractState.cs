using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntityState", Version = "1.0")]

namespace EfCoreTestSuite.CosmosDb.IntentGenerated.Entities.Polymorphic
{

    public partial class Poly_RootAbstract : IPoly_RootAbstract
    {

        public Guid Id { get; set; }

        public string AbstractField { get; set; }

        public string PartitionKey { get; set; }


        public Guid? Poly_RootAbstract_AggrId { get; set; }

        public virtual Poly_RootAbstract_Aggr Poly_RootAbstract_Aggr { get; set; }

        IPoly_RootAbstract_Aggr IPoly_RootAbstract.Poly_RootAbstract_Aggr
        {
            get => Poly_RootAbstract_Aggr;
            set => Poly_RootAbstract_Aggr = (Poly_RootAbstract_Aggr)value;
        }

        public virtual Poly_RootAbstract_Comp Poly_RootAbstract_Comp { get; set; }

        IPoly_RootAbstract_Comp IPoly_RootAbstract.Poly_RootAbstract_Comp
        {
            get => Poly_RootAbstract_Comp;
            set => Poly_RootAbstract_Comp = (Poly_RootAbstract_Comp)value;
        }


        public Guid? Poly_TopLevelId { get; set; }
    }
}
