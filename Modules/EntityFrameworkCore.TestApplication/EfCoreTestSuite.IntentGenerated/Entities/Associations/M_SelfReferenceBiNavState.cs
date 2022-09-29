using System;
using System.Collections.Generic;
using System.Linq;
using EfCoreTestSuite.IntentGenerated.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntityState", Version = "1.0")]

namespace EfCoreTestSuite.IntentGenerated.Entities.Associations
{

    public partial class M_SelfReferenceBiNav : IM_SelfReferenceBiNav
    {

        public Guid Id
        { get; set; }

        private string _selfRefBiNavAttr;

        public string SelfRefBiNavAttr
        {
            get { return _selfRefBiNavAttr; }
            set
            {
                _selfRefBiNavAttr = value;
            }
        }


        public Guid? M_SelfReferenceBiNavDstId { get; set; }

        public virtual M_SelfReferenceBiNav M_SelfReferenceBiNavDst
        { get; set; }

        IM_SelfReferenceBiNav IM_SelfReferenceBiNav.M_SelfReferenceBiNavDst
        {
            get => M_SelfReferenceBiNavDst;
            set => M_SelfReferenceBiNavDst = (M_SelfReferenceBiNav)value;
        }

        public virtual ICollection<M_SelfReferenceBiNav> M_SelfReferenceBiNavs
        { get; set; } = new List<M_SelfReferenceBiNav>();

        ICollection<IM_SelfReferenceBiNav> IM_SelfReferenceBiNav.M_SelfReferenceBiNavs
        {
            get => M_SelfReferenceBiNavs.CreateWrapper<IM_SelfReferenceBiNav, M_SelfReferenceBiNav>();
            set => M_SelfReferenceBiNavs = value.Cast<M_SelfReferenceBiNav>().ToList();
        }


    }
}
