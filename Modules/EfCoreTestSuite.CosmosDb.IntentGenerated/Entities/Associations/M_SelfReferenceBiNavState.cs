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

        public Guid? MSelfReferenceBiNavAssocationId { get; set; }

        public virtual M_SelfReferenceBiNav MSelfReferenceBiNavAssocation { get; set; }

        IM_SelfReferenceBiNav IM_SelfReferenceBiNav.MSelfReferenceBiNavAssocation
        {
            get => MSelfReferenceBiNavAssocation;
            set => MSelfReferenceBiNavAssocation = (M_SelfReferenceBiNav)value;
        }

        public virtual ICollection<M_SelfReferenceBiNav> MSelfReferenceBiNavs { get; set; } = new List<M_SelfReferenceBiNav>();

        ICollection<IM_SelfReferenceBiNav> IM_SelfReferenceBiNav.MSelfReferenceBiNavs
        {
            get => MSelfReferenceBiNavs.CreateWrapper<IM_SelfReferenceBiNav, M_SelfReferenceBiNav>();
            set => MSelfReferenceBiNavs = value.Cast<M_SelfReferenceBiNav>().ToList();
        }
    }
}