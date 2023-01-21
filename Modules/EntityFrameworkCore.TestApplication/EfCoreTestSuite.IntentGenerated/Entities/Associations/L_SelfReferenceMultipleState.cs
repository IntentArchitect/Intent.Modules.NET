using System;
using System.Collections.Generic;
using System.Linq;
using EfCoreTestSuite.IntentGenerated.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntityState", Version = "1.0")]

namespace EfCoreTestSuite.IntentGenerated.Entities.Associations
{

    public partial class L_SelfReferenceMultiple : IL_SelfReferenceMultiple
    {

        public Guid Id { get; set; }

        public string SelfRefMultipleAttr { get; set; }

        public virtual ICollection<L_SelfReferenceMultiple> LSelfReferenceMultiplesDst { get; set; } = new List<L_SelfReferenceMultiple>();

        ICollection<IL_SelfReferenceMultiple> IL_SelfReferenceMultiple.LSelfReferenceMultiplesDst
        {
            get => LSelfReferenceMultiplesDst.CreateWrapper<IL_SelfReferenceMultiple, L_SelfReferenceMultiple>();
            set => LSelfReferenceMultiplesDst = value.Cast<L_SelfReferenceMultiple>().ToList();
        }

        protected virtual ICollection<L_SelfReferenceMultiple> LSelfReferenceMultiplesSrc { get; set; }


    }
}
