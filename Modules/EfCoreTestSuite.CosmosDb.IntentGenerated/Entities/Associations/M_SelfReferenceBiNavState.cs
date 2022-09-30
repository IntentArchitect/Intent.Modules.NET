using System;
using System.Collections.Generic;
using System.Linq;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntityState", Version = "1.0")]

namespace EfCoreTestSuite.CosmosDb.IntentGenerated.Entities.Associations
{

    public partial class M_SelfReferenceBiNav : IM_SelfReferenceBiNav
    {

        public Guid Id { get; set; }

        public string PartitionKey { get; set; }

        public string SelfRefBiNavAttr { get; set; }


        public Guid? M_SelfReferenceBiNavAssocationId { get; set; }

        public virtual M_SelfReferenceBiNav M_SelfReferenceBiNavAssocation { get; set; }

        IM_SelfReferenceBiNav IM_SelfReferenceBiNav.M_SelfReferenceBiNavAssocation
        {
            get => M_SelfReferenceBiNavAssocation;
            set => M_SelfReferenceBiNavAssocation = (M_SelfReferenceBiNav)value;
        }

        public virtual ICollection<M_SelfReferenceBiNav> M_SelfReferenceBiNavs { get; set; } = new List<M_SelfReferenceBiNav>();

        ICollection<IM_SelfReferenceBiNav> IM_SelfReferenceBiNav.M_SelfReferenceBiNavs
        {
            get => M_SelfReferenceBiNavs.CreateWrapper<IM_SelfReferenceBiNav, M_SelfReferenceBiNav>();
            set => M_SelfReferenceBiNavs = value.Cast<M_SelfReferenceBiNav>().ToList();
        }


    }
}
